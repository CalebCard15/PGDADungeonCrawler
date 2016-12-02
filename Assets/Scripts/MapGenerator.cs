using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum ObjectType{Item, Enemy, Exit, Player, Prop, WallRock};

public class MapGenerator : MonoBehaviour {

	public GameManager gameManager;

	public int width;
	public int height;

	public int roomThresholdSize = 50;

	public int tilesPerEnemy = 500;
	public int tilesPerProp = 100;

	public string seed;
	public bool useRandomSeed;

	[Range(0,100)]
	public int randomFillPercent;
	public System.Random pseudoRandom;

	public GameObject wallRock;
	public GameObject exitTile;
	public GameObject player;

	public List<GameObject> itemList;
	public List<GameObject> enemyList;
	public List<GameObject> propList;


	int[,] map;

	void Start() {
		gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

		if (useRandomSeed) {
			seed = System.DateTime.Now.ToString();
		}
		pseudoRandom = new System.Random(seed.GetHashCode());

		GenerateMap();
	}

	void Update() {
		//if (Input.GetMouseButtonDown(0)) {
		//	GenerateMap();
		//}
	}

	void GenerateMap() {
		map = new int[width,height];
		RandomFillMap();

		for (int i = 0; i < 5; i ++) {
			SmoothMap();
		}

		ProcessMap ();

		int borderSize = 30;
		int[,] borderedMap = new int[width + borderSize * 2,height + borderSize * 2];

		for (int x = 0; x < borderedMap.GetLength(0); x ++) {
			for (int y = 0; y < borderedMap.GetLength(1); y ++) {
				if (x >= borderSize && x < width + borderSize && y >= borderSize && y < height + borderSize) {
					borderedMap[x,y] = map[x-borderSize,y-borderSize];
				}
				else {
					borderedMap[x,y] =1;
				}
			}
		}

		MeshGenerator meshGen = GetComponent<MeshGenerator>();
		meshGen.GenerateMesh(borderedMap, 1);
	}

	void ProcessMap() {
		List<List<Coord>> wallRegions = GetRegions (1);
		int wallThresholdSize = 50;

		foreach (List<Coord> wallRegion in wallRegions) {
			if (wallRegion.Count < wallThresholdSize) {
				foreach (Coord tile in wallRegion) {
					map[tile.tileX,tile.tileY] = 0;
				}
			}
		}

		List<List<Coord>> roomRegions = GetRegions (0);
	
		List<Room> survivingRooms = new List<Room> ();
		
		foreach (List<Coord> roomRegion in roomRegions) {
			if (roomRegion.Count < roomThresholdSize) {
				foreach (Coord tile in roomRegion) {
					map[tile.tileX,tile.tileY] = 1;
				}
			}
			else {
				survivingRooms.Add(new Room(roomRegion, map));
			}
		}
		survivingRooms.Sort ();
		survivingRooms [0].isMainRoom = true;
		survivingRooms [0].isAccessibleFromMainRoom = true;

		AddObjectsToDungeon(survivingRooms);
		ConnectClosestRooms (survivingRooms);
		CreateWallRocks();

	}

	//Add the rocks on the walls to try to soften the mesh
	void CreateWallRocks()
	{
		for(int i = 0; i < width; i++)
		{
			for(int j = 0; j < height; j++)
			{
				if(map[i,j] == 1 && GetSurroundingWallCount(i,j) < 7)
					PlaceObject(ObjectType.WallRock, CoordToWorldPoint(new Coord(i,j)));
			}
		}

	}

	//Actually places the object of type at the position given
	public void PlaceObject(ObjectType type, Vector3 position)
	{
		switch(type)
		{
			case ObjectType.Item:
				int itemNum = pseudoRandom.Next(0, itemList.Count);
				Instantiate(itemList[itemNum], position, itemList[itemNum].transform.rotation);
				break;
			case ObjectType.Enemy:
				int enemyNum = pseudoRandom.Next(0, enemyList.Count);
				Instantiate(enemyList[enemyNum], position, enemyList[enemyNum].transform.rotation);
				break;
			case ObjectType.Exit:
				Instantiate(exitTile, position, exitTile.transform.rotation);
				break;
			case ObjectType.Player:
				Player2D.instance.transform.position = position;
				break;
			case ObjectType.Prop:
				position.z = .01f;
				int propNum = pseudoRandom.Next(0, propList.Count);
				Instantiate(propList[propNum], position, propList[propNum].transform.rotation);
				break;
			case ObjectType.WallRock:
			Vector3 rotation = new Vector3(0,0,pseudoRandom.Next(0,360));
			Instantiate(wallRock, position, Quaternion.Euler(rotation));
				break;
		}

	}

	//Takes the list of surviving rooms and adds objects into them such as enemies and items
	void AddObjectsToDungeon(List<Room> rooms)
	{
		//Place the player in the map
		Vector3 playerPosition = CoordToWorldPoint(rooms[rooms.Count - 1].tiles[pseudoRandom.Next(0, rooms[rooms.Count - 1].tiles.Count)]);
		PlaceObject(ObjectType.Player, playerPosition);

		//Place the Exit tile in the map
		int roomNumber = pseudoRandom.Next(0, rooms.Count-1);
		Room exitRoom = rooms[roomNumber];

		int edgeTileNumber = pseudoRandom.Next(0, exitRoom.edgeTiles.Count);
		PlaceObject(ObjectType.Exit, CoordToWorldPoint(exitRoom.edgeTiles[edgeTileNumber]));

		//Place the enemies and the items and props in the map
		foreach(Room room in rooms)
		{
			print(room.tiles.Count);

			int numOfEnemies = room.tiles.Count/tilesPerEnemy;
			int numOfProps = room.tiles.Count/tilesPerProp;
			numOfEnemies *= gameManager.currentLevel;



			int tileNumber = pseudoRandom.Next(0, room.tiles.Count);
			PlaceObject(ObjectType.Item, CoordToWorldPoint(room.tiles[tileNumber]));



			for(int i = 0; i <= numOfProps; i++)
			{
				tileNumber = pseudoRandom.Next(0, room.tiles.Count);
				Vector3 propPosition = CoordToWorldPoint(room.tiles[tileNumber]);
				PlaceObject(ObjectType.Prop, propPosition);
			}

			for(int i = 0; i <= numOfEnemies; i++)
			{
				tileNumber = pseudoRandom.Next(0, room.tiles.Count);
				Vector3 enemyPosition = CoordToWorldPoint(room.tiles[tileNumber]);
				if(Vector3.Distance(playerPosition, enemyPosition) < 10)
				{
					continue;
				}
				PlaceObject(ObjectType.Enemy, enemyPosition);

			}
		}


			
	}

	void ConnectClosestRooms(List<Room> allRooms, bool forceAccessibilityFromMainRoom = false) {

		List<Room> roomListA = new List<Room> ();
		List<Room> roomListB = new List<Room> ();

		if (forceAccessibilityFromMainRoom)
		{
			foreach (Room room in allRooms)
			{
				if (room.isAccessibleFromMainRoom)
				{
					roomListB.Add (room);
				} 
				else
				{
					roomListA.Add (room);
				}
			}
		}
		else
		{
			roomListA = allRooms;
			roomListB = allRooms;
		}

		int bestDistance = 0;
		Coord bestTileA = new Coord ();
		Coord bestTileB = new Coord ();
		Room bestRoomA = new Room ();
		Room bestRoomB = new Room ();
		bool possibleConnectionFound = false;

		foreach (Room roomA in roomListA) {
			if (!forceAccessibilityFromMainRoom) {
				possibleConnectionFound = false;
				if (roomA.connectedRooms.Count > 0) {
					continue;
				}
			}

			foreach (Room roomB in roomListB) {
				if (roomA == roomB || roomA.IsConnected(roomB)) {
					continue;
				}
			
				for (int tileIndexA = 0; tileIndexA < roomA.edgeTiles.Count; tileIndexA ++) {
					for (int tileIndexB = 0; tileIndexB < roomB.edgeTiles.Count; tileIndexB ++) {
						Coord tileA = roomA.edgeTiles[tileIndexA];
						Coord tileB = roomB.edgeTiles[tileIndexB];
						int distanceBetweenRooms = (int)(Mathf.Pow (tileA.tileX-tileB.tileX,2) + Mathf.Pow (tileA.tileY-tileB.tileY,2));

						if (distanceBetweenRooms < bestDistance || !possibleConnectionFound) {
							bestDistance = distanceBetweenRooms;
							possibleConnectionFound = true;
							bestTileA = tileA;
							bestTileB = tileB;
							bestRoomA = roomA;
							bestRoomB = roomB;
						}
					}
				}
			}
			if (possibleConnectionFound && !forceAccessibilityFromMainRoom) {
				CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			}
		}

		if (possibleConnectionFound && forceAccessibilityFromMainRoom) {
			CreatePassage(bestRoomA, bestRoomB, bestTileA, bestTileB);
			ConnectClosestRooms(allRooms, true);
		}

		if (!forceAccessibilityFromMainRoom) {
			ConnectClosestRooms(allRooms, true);
		}
	}

	void CreatePassage(Room roomA, Room roomB, Coord tileA, Coord tileB) {
		Room.ConnectRooms (roomA, roomB);
		//Debug.DrawLine (CoordToWorldPoint (tileA), CoordToWorldPoint (tileB), Color.green, 100);

		List<Coord> line = GetLine (tileA, tileB);
		foreach (Coord c in line) {
			DrawCircle(c,5);
		}
	}

	void DrawCircle(Coord c, int r) {
		for (int x = -r; x <= r; x++) {
			for (int y = -r; y <= r; y++) {
				if (x*x + y*y <= r*r) {
					int drawX = c.tileX + x;
					int drawY = c.tileY + y;
					if (IsInMapRange(drawX, drawY)) {
						map[drawX,drawY] = 0;
					}
				}
			}
		}
	}

	List<Coord> GetLine(Coord from, Coord to) {
		List<Coord> line = new List<Coord> ();

		int x = from.tileX;
		int y = from.tileY;

		int dx = to.tileX - from.tileX;
		int dy = to.tileY - from.tileY;

		bool inverted = false;
		int step = Math.Sign (dx);
		int gradientStep = Math.Sign (dy);

		int longest = Mathf.Abs (dx);
		int shortest = Mathf.Abs (dy);

		if (longest < shortest) {
			inverted = true;
			longest = Mathf.Abs(dy);
			shortest = Mathf.Abs(dx);

			step = Math.Sign (dy);
			gradientStep = Math.Sign (dx);
		}

		int gradientAccumulation = longest / 2;
		for (int i =0; i < longest; i ++) {
			line.Add(new Coord(x,y));

			if (inverted) {
				y += step;
			}
			else {
				x += step;
			}

			gradientAccumulation += shortest;
			if (gradientAccumulation >= longest) {
				if (inverted) {
					x += gradientStep;
				}
				else {
					y += gradientStep;
				}
				gradientAccumulation -= longest;
			}
		}

		return line;
	}

	Vector3 CoordToWorldPoint(Coord tile) {
		return new Vector3 (-width / 2 + .5f + tile.tileX, -height / 2 + .5f + tile.tileY, 0);
	}

	List<List<Coord>> GetRegions(int tileType) {
		List<List<Coord>> regions = new List<List<Coord>> ();
		int[,] mapFlags = new int[width,height];

		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
					List<Coord> newRegion = GetRegionTiles(x,y);
					regions.Add(newRegion);

					foreach (Coord tile in newRegion) {
						mapFlags[tile.tileX, tile.tileY] = 1;
					}
				}
			}
		}

		return regions;
	}

	List<Coord> GetRegionTiles(int startX, int startY) {
		List<Coord> tiles = new List<Coord> ();
		int[,] mapFlags = new int[width,height];
		int tileType = map [startX, startY];

		Queue<Coord> queue = new Queue<Coord> ();
		queue.Enqueue (new Coord (startX, startY));
		mapFlags [startX, startY] = 1;

		while (queue.Count > 0) {
			Coord tile = queue.Dequeue();
			tiles.Add(tile);

			for (int x = tile.tileX - 1; x <= tile.tileX + 1; x++) {
				for (int y = tile.tileY - 1; y <= tile.tileY + 1; y++) {
					if (IsInMapRange(x,y) && (y == tile.tileY || x == tile.tileX)) {
						if (mapFlags[x,y] == 0 && map[x,y] == tileType) {
							mapFlags[x,y] = 1;
							queue.Enqueue(new Coord(x,y));
						}
					}
				}
			}
		}
		return tiles;
	}

	bool IsInMapRange(int x, int y) {
		return x >= 0 && x < width && y >= 0 && y < height;
	}


	void RandomFillMap() {
		

		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				if (x == 0 || x == width-1 || y == 0 || y == height -1) {
					map[x,y] = 1;
				}
				else {
					map[x,y] = (pseudoRandom.Next(0,100) < randomFillPercent)? 1: 0;
				}
			}
		}
	}

	void SmoothMap() {
		for (int x = 0; x < width; x ++) {
			for (int y = 0; y < height; y ++) {
				int neighbourWallTiles = GetSurroundingWallCount(x,y);

				if (neighbourWallTiles > 4)
					map[x,y] = 1;
				else if (neighbourWallTiles < 4)
					map[x,y] = 0;

			}
		}
	}

	int GetSurroundingWallCount(int gridX, int gridY) {
		int wallCount = 0;
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++) {
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++) {
				if (IsInMapRange(neighbourX,neighbourY)) {
					if (neighbourX != gridX || neighbourY != gridY) {
						wallCount += map[neighbourX,neighbourY];
					}
				}
				else {
					wallCount ++;
				}
			}
		}

		return wallCount;
	}

	struct Coord {
		public int tileX;
		public int tileY;

		public Coord(int x, int y) {
			tileX = x;
			tileY = y;
		}
	}


	class Room : IComparable<Room> {
		public List<Coord> tiles;
		public List<Coord> edgeTiles;
		public List<Room> connectedRooms;
		public int roomSize;
		public bool isAccessibleFromMainRoom;
		public bool isMainRoom;

		public Room() {
		}

		public Room(List<Coord> roomTiles, int[,] map) {
			tiles = roomTiles;
			roomSize = tiles.Count;
			connectedRooms = new List<Room>();

			edgeTiles = new List<Coord>();
			foreach (Coord tile in tiles) {
				for (int x = tile.tileX-1; x <= tile.tileX+1; x++) {
					for (int y = tile.tileY-1; y <= tile.tileY+1; y++) {
						if (x == tile.tileX || y == tile.tileY) {
							if (map[x,y] == 1) {
								edgeTiles.Add(tile);
							}
						}
					}
				}
			}
		}

		public void SetAccessibleFromMainRoom() {
			if (!isAccessibleFromMainRoom) {
				isAccessibleFromMainRoom = true;
				foreach (Room connectedRoom in connectedRooms) {
					connectedRoom.SetAccessibleFromMainRoom();
				}
			}
		}

		public static void ConnectRooms(Room roomA, Room roomB) {
			if (roomA.isAccessibleFromMainRoom) {
				roomB.SetAccessibleFromMainRoom ();
			} else if (roomB.isAccessibleFromMainRoom) {
				roomA.SetAccessibleFromMainRoom();
			}
			roomA.connectedRooms.Add (roomB);
			roomB.connectedRooms.Add (roomA);
		}

		public bool IsConnected(Room otherRoom) {
			return connectedRooms.Contains(otherRoom);
		}

		public int CompareTo(Room otherRoom) {
			return otherRoom.roomSize.CompareTo (roomSize);
		}
	}

}
