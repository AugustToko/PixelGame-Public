using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// needed for List<>
//needed for the Select() in the lambda statement

//needed for drawing the Map onto UI

namespace PixelGameAssets.Scripts.Unused
{
    public class DungeonGenerator : MonoBehaviour
    {
        //some 2D dungeon Generators converted to C# by Marrt
        //Version 0.31, 2015-10-21
        //Usage:

        //0. Add the Extension.cs (needed for a specific Hashsets conversion) to your Project

        //1. Slap this script onto a GameObject and call one of those from anywhere:
        //- "DungeonGenerator.instance.GenerateHauberkDungeon(int width, int height)" //you can overwrite some or all parameters, but only in order
        //- "DungeonGenerator.instance.GeneratePerlinDungeon(int width, int height)"
        //- "DungeonGenerator.instance.GenerateCaveDungeon(int width, int height)"
        //
        //2. Be sure to have a canvas-GameObject in your Hierarchy for the map to be created and drawn
        //- "CreateMap()" is called in Awake, after that it is only Updated
        //- "UpdateMap(ref _dungeon)" is called on the end of above functions
        //- a button is added to the map that toggles region view mode for the hauberk dungeon
        //- use "DungeonGenerator.instance.ShowTransformOnMap(Transform actor, float gridX, float gridY)" to draw a red point onto the map
        //
        //3. Using the data
        //- The Tilegrid can be accessed by the "Tile[,] dungeon" variable AFTER one of the Generate functios have been called, Tile enum is defined globally at the Top of this script
        //- write your costum instantiate scripts to create the level, i do it like that:
        /*	
     * 	private	Transform mapParent;
     * 	public void GenerateByGrid(Tile[,] grid){
     * 		
     * 		int xSize = grid.GetLength(0);
     * 		int ySize = grid.GetLength(1);
     * 		Vector3 midOff = new Vector3(0F, -yOff/2F, 0F);	 * 				
     * 		mapParent = new GameObject().transform;
     * 		mapParent.name = "DynamicBlocks";
     * 		
     * 		for(int x = 0; x < xSize; x++){				
     * 			for(int y = 0; y < ySize; y++){				
     * 				switch(grid[x,y]){
     * 					default:																					break;
     * 					case Tile.Floor:		CreateBlock( new Vector3(xOff *x, yOff * 0F, zOff * y) +midOff);	break;
     * 					case Tile.RoomFloor:	CreateBlock( new Vector3(xOff *x, yOff * 0F, zOff * y) +midOff);	break;
     * 					case Tile.Wall:			CreateBlock( new Vector3(xOff *x, yOff * 0F, zOff * y) +midOff);
     * 											CreateBlock( new Vector3(xOff *x, yOff * 1F, zOff * y) +midOff);	
     * 																												break;					
     * 				}			
     * 			}				
     * 		}
     * 	}
     * 	private	void CreateBlock(Vector3 pos){
     * 		GameObject block		= Instantiate(dynBlockPrefab, pos, Quaternion.identity) as GameObject;
     * 		block.transform.parent	= mapParent;
     * 	}
    */

        //KNOWN ISSUES:
        //- style: some class variables have underscores - or not
        //- style: Why cram everything into a single script? I don't know, lack of sophisticated coding patterns from my side i guess, i am open to suggestions
        //- Cave: I carelessly replaced randoms with Random.Range, there may be some Range errors since Random.Range(int,int) will always always be lower that the second int
        //- Implementation: i just learned about the more elaborate Datatypes like HashSets and Dictionaries through the Hauberk conversion, errors may be present

        public static DungeonGenerator Instance;

        public static Tile[,] _dungeon; //2D-Array that stores the current type of a tile
        public static int[,] _regions; //2D-Array

        public int _dungeonWidth = 81;
        public int _dungeonHeight = 41;

        public bool _randomSeedOn = false;
        public int _seed; //random seed for map generation

        public bool addMapToggle = true; //toggle mapview between _regions and _dungeon

        public static void setTile(IntVector2 pos, Tile tile)
        {
            _dungeon[pos.x, pos.y] = tile;
        } //OCCASIONAL ERROR

        public Tile getTile(IntVector2 pos)
        {
            return _dungeon[pos.x, pos.y];
        }

        private void Awake()
        {
            Instance = this;

            cardinal = new List<IntVector2>
            {
                new IntVector2(0, 1), new IntVector2(1, 0), new IntVector2(0, -1), new IntVector2(-1, 0)
            };
            //North
            //East
            //South
            //West	

            CreateMap();
        }

        private IntVector2 FindFloorInDungeon(Dungeon dungeon)
        {
            var xSize = dungeon.tiles.GetLength(0);
            var ySize = dungeon.tiles.GetLength(1);

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (dungeon.tiles[x, y] == Tile.Floor)
                    {
                        return new IntVector2(x, y);
                    }
                }
            }

            return new IntVector2(0, 0);
        }

        // ███████████████████████████████████████████████████████████████████████████

        #region RoomTemplates generation

        //Main function body
        public Dungeon GenerateRoomTemplateDungeon(DungeonParameters dParams = null, string previousDungeon = "")
        {
            //default dungeon Parameters if non are supplied
            if (dParams == null)
            {
                PortalTemplate entrancePortal = new PortalTemplate("na", PathRole.Entrance, PortalDirection.South);
                PortalTemplate exitPortal = new PortalTemplate("na", PathRole.Exit, PortalDirection.Any);
                List<PortalTemplate> defaultPortals = new List<PortalTemplate>() {entrancePortal, exitPortal};
                dParams = new DungeonParameters(80, 50, 11, defaultPortals);
            }

            int placeTrials = 10; // if this is exceeded we abort
            int graphTrials =
                10; // if the graph won't work the first time after placement, we have to go back completely
            int portalTrials = 10; // if portals cannot be placed, we have to go back completely

            int placeAttempt = 1; // if this is exceeded we abort
            int graphAttempt =
                1; // if the graph won't work the first time after placement, we have to go back completely
            int portalAttempt = 1; // if portals cannot be placed, we have to go back completely

            bool success = false;

            RoomTemplateDungeonPath path = null;
            RoomDungeon rDungeon = null;

            _dungeonWidth = dParams.width;
            _dungeonHeight = dParams.height;

            int printAbleSeed = 000; //smaller memorizeable seed for debugging

            if (_randomSeedOn)
            {
                Random.seed = this._seed;
            }
            else
            {
                //	create random base seed to make it visible for debugging			
                //	printAbleSeed = Random.Range(int.MinValue,int.MaxValue);	//full range
                printAbleSeed = Random.Range(0, 999); //Debug: 3 digits, easier to remember
                print("Level Seed: <" + printAbleSeed + ">");
                Random.seed = printAbleSeed;
            }


            //	//RANDOM ROOMS
            //	PerformanceMeter.StartTimeLog("generate templates:\t\t");		
            //	roomTemplates = GenerateRoomTemplates();

            //PREDEFINED ROOMS
            PerformanceMeter.StartTimeLog("read templates:\t\t\t");
            roomTemplates = ParseTexture(0);
            PerformanceMeter.EndTimeLog();

            PortalTemplate usedEntrance = dParams.portalTemplates.Find(portal => portal.pathrole == PathRole.Entrance);
            bool firstIsEntrance = usedEntrance.direction != PortalDirection.Any &&
                                   usedEntrance.direction != PortalDirection.None;

            //BEGIN TRIAL
            while (portalAttempt < portalTrials)
            {
                while (graphAttempt < graphTrials)
                {
                    //PLACEMENT: (fails if roomcount is to low)
                    while (placeAttempt < placeTrials)
                    {
                        PerformanceMeter.StartTimeLog("room placement try#" + (placeAttempt + 1) + ":\t");
                        success = RoomPlacementTrial(firstIsEntrance, usedEntrance, dParams.minimumRoomCount);
                        PerformanceMeter.EndTimeLog();

                        if (success)
                        {
                            break;
                        }
                        else
                        {
                            placeAttempt++;
                        }
                    }

                    //Debug, if graph fails, insert "UpdateMap(ref _dungeon);" to see how the room looked like

                    //GRAPH:	(fails if mandatory path is to short)

                    //create Edges between doors of the same room
                    foreach (Room room in placedRooms)
                    {
                        CreateDoorEdgesWithinRoom(room);
                    }

                    PerformanceMeter.StartTimeLog("dungeon graph:\t\t\t");
                    //find most distant rooms(only marks on map for now), e.g. to place entrance and exit there for least sidetracks to minimize backtracking
                    path = FindMostDistantRooms(firstIsEntrance);
                    success = path != null;
                    PerformanceMeter.EndTimeLog();

                    if (success)
                    {
                        break;
                    }
                    else
                    {
                        graphAttempt++;
                    }
                }


                //INIT ROOMDUNGEON:
                rDungeon = new RoomDungeon(dParams, _dungeon, _regions, placedRooms);
                rDungeon.portals = new List<Portal>();
                foreach (PortalTemplate template in rDungeon.parameters.portalTemplates)
                {
                    rDungeon.portals.Add(new Portal(template));
                }

                //CREATE PORTALS: (fails if edge portals cannot reach the map bound)				
                PerformanceMeter.StartTimeLog("portal placement try#" + 00 + ":\t");
                success = PortalPlacementTrial(rDungeon, previousDungeon);
                PerformanceMeter.EndTimeLog();

                if (success)
                {
                    break;
                }
                else
                {
                    portalAttempt++;
                }
            }


            FillVoids(rDungeon);

            //INIT REGIONS
            for (int x = 1; x < _dungeonWidth - 1; x++)
            {
                for (int y = 1; y < _dungeonHeight - 1; y++)
                {
                    switch (_dungeon[x, y])
                    {
                        case Tile.None:
                            _regions[x, y] = -1000;
                            break;
                        case Tile.Wall:
                            _regions[x, y] = -100;
                            break;
                        case Tile.Door:
                            _regions[x, y] = -10;
                            break;
                    }
                }
            }


            PerformanceMeter.AddLog("Finished<" + printAbleSeed + ">, Attempts:[" + placeAttempt + "," + graphAttempt +
                                    "," + portalAttempt + "]");
            PerformanceMeter.AddLog("");
            UpdateMap(ref _dungeon);

            //Color Path for debugging
            ColorRoomDungeonSolutionPath(path);

            //ToggleMap();

            //fill base Tiles		
            return rDungeon;
        }

        private bool RoomPlacementTrial(bool firstIsEntrance, PortalTemplate usedEntrance, int minimumRoomCount)
        {
            //reset used variables
            _dungeon = new Tile[_dungeonWidth, _dungeonHeight];
            _regions = new int [_dungeonWidth, _dungeonHeight];
            currentRoom = 0;
            placedRooms = new List<Room>();

            //create first room in a way that it will have access to the entrance portalat the corresponding border
            if (firstIsEntrance)
            {
                Room firstRoom = CreateRoomFromTemplate(roomTemplates[Random.Range(0, roomTemplates.Count)]);

                if (firstRoom.potentialDoors.FirstOrDefault(door =>
                        door.doorDir == usedEntrance.direction.GetDoorDirection()) == null)
                {
                    print("RoomPlacementTrial: no door found");
                    return false;
                }

                int bridge =
                    3; // bridge length, should be so small that no room can be placed between this room and border

                int min = 0 + bridge;
                int maxX = _dungeonWidth - firstRoom.width - bridge;
                int maxY = _dungeonHeight - firstRoom.height - bridge;
                int offX = Random.Range(min, maxX);
                int offY = Random.Range(min, maxY);

                switch (usedEntrance.direction)
                {
                    case PortalDirection.North:
                        offY = maxY;
                        break;
                    case PortalDirection.East:
                        offX = maxX;
                        break;
                    case PortalDirection.South:
                        offY = min;
                        break;
                    case PortalDirection.West:
                        offX = min;
                        break;
                }

                CarveRoomIntoDungeon(firstRoom, offX, offY);
            }


            //Placement
            for (int i = 0; i < roomTries; i++)
            {
                int roomNumber = Random.Range(0, roomTemplates.Count);
                TryToPlaceRoom(roomTemplates[roomNumber]);
            }

            //10 tries to get a valid Dungeon roomCount		
            if (placedRooms.Count < minimumRoomCount)
            {
                //PerformanceMeter.StartTimeLog("to few rooms: ["+placedRooms.Count+"]\t\t");PerformanceMeter.EndTimeLog();
                print("RoomPlacementTrial: couldn't place enough rooms, try again");
                return false;
            }
            else
            {
                return true;
            }
        }

        private bool PortalPlacementTrial(RoomDungeon rDungeon, string previousDungeonID)
        {
            bool success = false;

            foreach (Portal portal in rDungeon.portals)
            {
                success = false;
                foreach (Room room in rDungeon.rooms)
                {
                    bool roomValid = portal.template.pathrole == room.pathrole;

                    if (roomValid)
                    {
                        //try to carve portal entry, entrance should always be possible since first room is placed to make it so
                        if (portal.template.direction == PortalDirection.Any)
                        {
                            //try opposite of entrance first
                            portal.direction = rDungeon.portals[0].direction.GetOpposite();
                            success = TryToCreatePortalBridge(portal, rDungeon, room);
                            if (!success)
                            {
                                portal.direction = portal.direction.GetRotated();
                                success = TryToCreatePortalBridge(portal, rDungeon, room);
                            }

                            if (!success)
                            {
                                portal.direction = portal.direction.GetRotated();
                                success = TryToCreatePortalBridge(portal, rDungeon, room);
                            }

                            if (!success)
                            {
                                portal.direction = portal.direction.GetRotated();
                                success = TryToCreatePortalBridge(portal, rDungeon, room);
                            }
                        }
                        else
                        {
                            //use specified direction
                            success = TryToCreatePortalBridge(portal, rDungeon, room);
                        }

                        if (success)
                        {
                            //print("successfully created portal:\n"+portal.GetPortalInfo());

                            if (portal.template.stringID == previousDungeonID)
                            {
                                rDungeon.entrance = portal.position;
                                rDungeon.entranceSet = true;
                            }

                            break; //finished this portal
                        }
                    }

                    //		if(room.name == "Entrance"){
                    //			rDungeon.entrance		= FindFloorInRoom(room,3);
                    //			rDungeon.entranceSet	= true;		
                    //		}else if(room.name == "Exit"){
                    //			rDungeon.exit			= FindFloorInRoom(room,3);
                    //			rDungeon.exitSet		= true;
                    //		}
                }

                if (!success)
                {
                    //	print("FAILED to create portal:\n"+portal.GetPortalInfo());
                    return success;
                    //	ColorRoomDungeonSolutionPath( path );
                }
            }

            if (rDungeon.entranceSet == false)
            {
                print("Error, no corresponding portal found, fallign back to default entrance");
                Portal entrance = rDungeon.portals.FirstOrDefault(entr => entr.template.pathrole == PathRole.Entrance);
                rDungeon.entrance = entrance.position;
                rDungeon.entranceSet = true;
            }

            return success;
        }


        private bool TryToCreatePortalBridge(Portal portal, RoomDungeon rDungeon, Room targetRoom)
        {
            int xSize = rDungeon.tiles.GetLength(0);
            int ySize = rDungeon.tiles.GetLength(1);

            DoorDirection dir = portal.direction.GetDoorDirection();
            IntVector2 offset = portal.direction.GetIntVector2();

            //Find all compatible Doors
            List<Door> compatibleDoors = targetRoom.potentialDoors.FindAll((x) => x.doorDir == dir);

            //	print ("Doordir "+compatibleDoors.Count);

            foreach (Door door in compatibleDoors)
            {
                List<IntVector2> doorPoints = door.GetOccupiedTilesCoordinates;
                List<IntVector2> bridgeTiles = new List<IntVector2>();
                bool reachedBorder = true;

                IntVector2 curPos = new IntVector2(-1, -1);

                foreach (IntVector2 point in doorPoints)
                {
                    //get position
                    curPos = targetRoom.GetOrigin + point; //east			
                    curPos += offset;


                    //until a bound is hit or break condition is found
                    while (IntPointInBounds(curPos, xSize, ySize))
                    {
                        //	print (curPos.x+","+curPos.y);

                        if (rDungeon.tiles[curPos.x, curPos.y] != Tile.None)
                        {
                            reachedBorder = false;
                            break;
                        } //we hit an obstacle
                        else
                        {
                            bridgeTiles.Add(curPos);
                        } //we didn't save coordinate for later, it may be carved out

                        curPos += offset;
                    }
                }

                //Carve Changes
                if (reachedBorder)
                {
                    targetRoom.MakeDoorActual(door);

                    //carve door
                    foreach (IntVector2 point in doorPoints)
                    {
                        IntVector2 pointD =
                            new IntVector2(targetRoom.x + point.x, targetRoom.y + point.y); //point in Dungeon
                        if (IntPointInBounds(pointD, xSize, ySize))
                        {
                            rDungeon.tiles[pointD.x, pointD.y] = Tile.Door;
                        }
                        else
                        {
                            print("error" + pointD.x + "," + pointD.y);
                        }
                    }

                    //carve bridge
                    foreach (IntVector2 bridgeTile in bridgeTiles)
                    {
                        rDungeon.tiles[bridgeTile.x, bridgeTile.y] = Tile.Floor;
                    }

                    //add bridge walls
                    foreach (IntVector2 bridgeTile in bridgeTiles)
                    {
                        foreach (IntVector2 v2 in Directions8)
                        {
                            IntVector2 point = new IntVector2(bridgeTile.x + v2.x, bridgeTile.y + v2.y);

                            if (IntPointInBounds(point, xSize, ySize) && rDungeon.tiles[point.x, point.y] == Tile.None)
                            {
                                rDungeon.tiles[point.x, point.y] = Tile.Wall;
                            }
                        }
                    }


                    //entrance = last position before hitting border
                    if (bridgeTiles.Count > 0)
                    {
                        portal.position = bridgeTiles[bridgeTiles.Count - 1];

                        //if border is reached immideately
                    }
                    else if (curPos != new IntVector2(-1, -1))
                    {
                        //curPos = curPos.Clamp(IntVector2.zero, new IntVector2(xSize-1,ySize-1));
                        portal.position = curPos;
                        print("borderReached immidiately");
                        //error curPos never set
                    }
                    else
                    {
                        print("error, curPos never set (this should never happen)");
                    }

                    return true;
                }
            }

            return false;
        }

        private static bool IntPointInBounds(IntVector2 point, int width, int height)
        {
            if (point.x < 0 || point.y < 0 || point.x > width - 1 || point.y > height - 1)
            {
                return false;
            }

            return true;
        }

        private void FillVoids(RoomDungeon rDungeon)
        {
            int xSize = rDungeon.tiles.GetLength(0);
            int ySize = rDungeon.tiles.GetLength(1);

            PortalDirection dir = PortalDirection.West;

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (rDungeon.tiles[x, y] == Tile.None)
                    {
                        switch (dir)
                        {
                            case PortalDirection.North:
                                if (y < ySize / 2)
                                {
                                    rDungeon.tiles[x, y] = Tile.Wall;
                                }

                                break;
                            case PortalDirection.East:
                                if (x > xSize / 2)
                                {
                                    rDungeon.tiles[x, y] = Tile.Wall;
                                }

                                break;
                            case PortalDirection.South:
                                if (y > ySize / 2)
                                {
                                    rDungeon.tiles[x, y] = Tile.Wall;
                                }

                                break;
                            case PortalDirection.West:
                                if (x < xSize / 2)
                                {
                                    rDungeon.tiles[x, y] = Tile.Wall;
                                }

                                break;
                        }
                    }
                }
            }
        }


        [Header("	RoomTemplatePlacement Dungeon")]

        //Dungeon created by placeing room templates connected by doors, by Marrt (expect obvious inanity)
        private List<RoomTemplate> roomTemplates;

        private List<Room> placedRooms;

        [Range(1, 250)] public int roomTries = 250; //amount of placement tries

        [Range(3, 30)] public int minRoomSize = 8; //minimal size of room
        [Range(3, 30)] public int maxRoomSize = 18; //maximum size of NON-Overlayed rooms

        [Range(1, 250)] public int randomTemplateCount = 100; //amount of templates to generate

        [Range(0, 100)] public int wrinkledRoomProbability = 0; //chance that rooms will get some wrinkling treatment

        [Range(0, 10)] public int overlayCount = 3;

        [Range(0, 100)] public int overlayChance = 50;

        public bool doors1x1Allowed = false; //only add roomn if 2x2 door can be placed	
        public bool doors2x2Allowed = false; //search for potential doors with length 2 where possible

        private int currentRoom;


        public bool TryToPlaceRoom(RoomTemplate roomTemplate)
        {
            //CHANGE! start with placed room and try to add to it

            int xSizeDungeon = _dungeon.GetLength(0);
            int ySizeDungeon = _dungeon.GetLength(1);

            int xSizeRoom = roomTemplate.tiles.GetLength(0);
            int ySizeRoom = roomTemplate.tiles.GetLength(1);

            int xMax = xSizeDungeon - xSizeRoom;
            int yMax = ySizeDungeon - ySizeRoom;

            bool collision = false;

            //try
            if (xMax < 0 || yMax < 0)
            {
                print("roomTemplate won't fit");
            }
            else
            {
                Room newRoom = null;

                int offX;
                int offY;

                //for first room placement is random, first room needs no collision tests
                if (placedRooms.Count == 0)
                {
                    //create new room from template, otherwise we would alter the template itself				
                    newRoom = CreateRoomFromTemplate(roomTemplate);

                    //random offset
                    offX = Random.Range(0, xMax);
                    offY = Random.Range(0, yMax);
                }
                else
                {
                    //choose a door from a placed room and try to place a room with a fitting door next to it
                    Door jackDoor; //a single door of a placed room
                    List<Door> potentialFits = new List<Door>(); //fitting door of the new Room

                    Room placedRoom = placedRooms[Random.Range(0, placedRooms.Count)]; //random already placed room				
                    if (placedRoom.potentialDoors.Count == 0)
                    {
                        return false;
                    } //has room remaining potential doors?

                    jackDoor = placedRoom.potentialDoors[
                        Random.Range(0,
                            placedRoom.potentialDoors.Count)]; //random unused door as socket for a plug door

                    //get needed direction and set vertical flag
                    DoorDirection neededDir;
                    bool verticalDoor = true;
                    if (jackDoor.doorDir == DoorDirection.North)
                    {
                        neededDir = DoorDirection.South;
                    }
                    else if (jackDoor.doorDir == DoorDirection.East)
                    {
                        neededDir = DoorDirection.West;
                        verticalDoor = false;
                    }
                    else if (jackDoor.doorDir == DoorDirection.South)
                    {
                        neededDir = DoorDirection.North;
                    }
                    else
                    {
                        neededDir = DoorDirection.East;
                        verticalDoor = false;
                    }

                    //parse all potential doors for fits
                    foreach (Door door in roomTemplate.potentialDoors)
                    {
                        if (door.doorDir == neededDir && door.doorLength == jackDoor.doorLength)
                        {
                            potentialFits.Add(door);
                        }
                    }

                    if (potentialFits.Count == 0)
                    {
                        return false;
                    } //no fit

                    Door plugDoor = potentialFits[Random.Range(0, potentialFits.Count)];

                    //only 2x2 doors?
                    if (!doors1x1Allowed)
                    {
                        if (plugDoor.doorLength != 2 && jackDoor.doorLength != 2)
                        {
                            return false;
                        }
                    }

                    //Doors match, now get the needed room offset to place 1: "door in door" if door lenth = 1, or 2: "door on door" of 2				
                    IntVector2 doorOffset = jackDoor.doorLength == 2
                        ? new IntVector2(verticalDoor ? 0 : (neededDir == DoorDirection.East ? -1 : +1),
                            verticalDoor ? (neededDir == DoorDirection.North ? -1 : +1) : 0)
                        : new IntVector2(0, 0);

                    offX = placedRoom.x + jackDoor.x - plugDoor.x + doorOffset.x;
                    offY = placedRoom.y + jackDoor.y - plugDoor.y + doorOffset.y;
                    //			
                    if (offX < 0 || offX > xMax || offY < 0 || offY > yMax)
                    {
                        return false;
                    } //room outside of dungeon area

                    //test if room can fit here, if we start from >0 we essentially allow a overlap
                    for (int x = 0; x < xSizeRoom; x++)
                    {
                        for (int y = 0; y < ySizeRoom; y++)
                        {
                            //OLD: completely bordered rooms only: Collision happens when we hit a floor Tile of another room with a Wall or Floor, potential doors are walls at the state, carve them later					
                            //	collision = (_dungeon[x +offX,y +offY] == Tile.Floor) && roomTemplate.tiles[x,y] != Tile.None;

                            //New: One must be None, but walls can fuse
                            collision =
                                !((_dungeon[x + offX, y + offY] == Tile.None) ||
                                  roomTemplate.tiles[x, y] == Tile.None) ||
                                (_dungeon[x + offX, y + offY] == Tile.Wall && roomTemplate.tiles[x, y] == Tile.Wall);

                            //	collision = (_dungeon[x +offX,y +offY] == Tile.Floor || _dungeon[x +offX,y +offY] == Tile.RoomFloor) && roomTemplate.tiles[x,y] != Tile.None;		//Debug

                            if (collision)
                            {
                                break;
                            }
                        }

                        if (collision)
                        {
                            break;
                        }
                    }

                    //doors are carved into the new room directly before placement
                    if (!collision)
                    {
                        //create actual room

                        newRoom = CreateRoomFromTemplate(roomTemplate);

                        int plugDoorIndex = roomTemplate.potentialDoors.FindIndex(x => x == plugDoor);
                        plugDoor = newRoom.potentialDoors[
                            plugDoorIndex]; //refresh: reference the plugdoor in the room, not the roomTemplate

                        newRoom.tiles[plugDoor.x, plugDoor.y] = Tile.Door;
                        placedRoom.tiles[jackDoor.x, jackDoor.y] = Tile.Door;

                        _dungeon[placedRoom.x + jackDoor.x, placedRoom.y + jackDoor.y] =
                            Tile.Door; //retroactively carve door into dungeon, placedRoom has been carved before

                        //2x2 Door
                        if (jackDoor.doorLength > 1)
                        {
                            newRoom.tiles[plugDoor.x + (verticalDoor ? 1 : 0), plugDoor.y + (verticalDoor ? 0 : 1)] =
                                Tile.Door;
                            placedRoom.tiles[jackDoor.x + (verticalDoor ? 1 : 0), jackDoor.y + (verticalDoor ? 0 : 1)] =
                                Tile.Door;
                            _dungeon[placedRoom.x + jackDoor.x + (verticalDoor ? 1 : 0),
                                placedRoom.y + jackDoor.y + (verticalDoor ? 0 : 1)] = Tile.Door;
                        }


                        //GRAPH, make the rooms know about each other by referencing them in their adjacent lists
                        placedRoom.connected.Add(newRoom);
                        newRoom.connected.Add(placedRoom);

                        //ADD doors to the doorLists					
                        //		placedRoom.doors.Add(jackDoor);
                        //		newRoom.doors.Add(plugDoor);
                        //REMOVE doors from potentialDoors					
                        //		placedRoom.potentialDoors.Remove(jackDoor);	//door is no longer potential but actual
                        //		newRoom.potentialDoors.Remove(plugDoor);

                        //make doors actual
                        placedRoom.MakeDoorActual(jackDoor);
                        newRoom.MakeDoorActual(plugDoor);


                        //GRAPH, add the door transition to the door graph for weighted pathfinding, distance is 10 if doors placed onto each other, and 10 if next to each other (2x2 doors). 0 distance would break dijkstra!!!
                        jackDoor.connections.Add(new Edge(plugDoor, jackDoor.doorLength * 10));
                        plugDoor.connections.Add(new Edge(jackDoor, jackDoor.doorLength * 10));
                        jackDoor.owner = placedRoom;
                        plugDoor.owner = newRoom;
                        //GRAPH, connections between doors within a room are made when placement is finished because we cannot know which potential doors will become actual now
                    }
                }

                //write room into dungeon if it has not collided
                if (!collision)
                {
                    CarveRoomIntoDungeon(newRoom, offX, offY);
                }
            }

            return !collision;
        }

        private void CarveRoomIntoDungeon(Room room, int offX, int offY)
        {
            int xSizeRoom = room.width;
            int ySizeRoom = room.height;

            //carve room into _dungeon
            for (int x = 0; x < xSizeRoom; x++)
            {
                for (int y = 0; y < ySizeRoom; y++)
                {
                    if (room.tiles[x, y] != Tile.None)
                    {
                        //copy everything except Tile.None into dungeon							
                        _dungeon[x + offX, y + offY] = room.tiles[x, y];
                        if (room.tiles[x, y] == Tile.Floor)
                        {
                            _regions[x + offX, y + offY] = currentRoom;
                        } //set region too					
                    }
                }
            }

            room.index = currentRoom;
            currentRoom++;

            room.x = offX;
            room.y = offY;

            placedRooms.Add(room);
            room.name = "Room " + placedRooms.Count.ToString("00");
        }

        private Room CreateRoomFromTemplate(RoomTemplate roomTemplate)
        {
            int xSizeRoom = roomTemplate.tiles.GetLength(0);
            int ySizeRoom = roomTemplate.tiles.GetLength(1);

            Room room;

            //copy array ( no reference to old array )
            Tile[,] roomTemplateTilesCopy = new Tile[xSizeRoom, ySizeRoom];
            System.Array.Copy(roomTemplate.tiles, roomTemplateTilesCopy, xSizeRoom * ySizeRoom);

            room = new Room(roomTemplateTilesCopy, 0, 0, new List<Door>());

            //copy potential doors, don't use structs because doors are used as nodes later on and must be passed by ref then		
            foreach (Door door in roomTemplate.potentialDoors)
            {
                //Door door in roomTemplate.potentialDoors){
                //copy potentialDoor ( no reference to old door )		
                room.potentialDoors.Add(new Door(door.doorDir, door.doorLength, door.x, door.y,
                    room)); //copy the door object			
            }

            room.spawners = new List<IntVector2>();

            //copy spawners
            foreach (IntVector2 v2 in roomTemplate.spawners)
            {
                //Door door in roomTemplate.potentialDoors){
                //copy potentialDoor ( no reference to old door )		
                room.spawners.Add(new IntVector2(v2.x, v2.y)); //copy the door object			
            }

            return room;
        }


        private void TryMoreDoors()
        {
        }

        private void BuildDungeonGraph()
        {
        }

        private IntVector2 FindFloorInRoom(Room room, int min)
        {
            int xSize = room.tiles.GetLength(0);
            int ySize = room.tiles.GetLength(1);

            for (int x = min; x < xSize; x++)
            {
                for (int y = min; y < ySize; y++)
                {
                    if (room.tiles[x, y] == Tile.Floor)
                    {
                        return new IntVector2(x, y) + new IntVector2(room.x, room.y);
                    }
                }
            }

            return new IntVector2(0, 0);
        }

        //Graph stuff, find shortest Path between al room pairs and mark the longest
        private RoomTemplateDungeonPath FindMostDistantRooms(bool firstIsEntrance)
        {
            if (placedRooms.Count < 2)
            {
                return null;
            }

            Room room1 = null;
            Room room2 = null;

            RoomTemplateDungeonPath longestPath = new RoomTemplateDungeonPath(); //inits with length 0

            //check each room pair once (= roomsize * roomsize/2 times FindPath())

            int max = firstIsEntrance ? 1 : placedRooms.Count; //search between all rooms or only from first

            for (int i1 = 0; i1 < max; i1++)
            {
                for (int i2 = i1 + 1; i2 < placedRooms.Count; i2++)
                {
                    RoomTemplateDungeonPath path = FindPath(placedRooms[i1], placedRooms[i2]);
                    if (path.length > longestPath.length)
                    {
                        longestPath = path;
                        room1 = placedRooms[i1];
                        room2 = placedRooms[i2];
                    }
                }
            }

            room1.name = "Entrance";
            room2.name = "Exit";

            foreach (Room room in placedRooms)
            {
                if (room == longestPath.rooms[0])
                {
                    room.pathrole = PathRole.Entrance;
                }
                else if (room == longestPath.rooms[longestPath.rooms.Count - 1])
                {
                    room.pathrole = PathRole.Exit;
                }
                else if (longestPath.rooms.Contains(room))
                {
                    room.pathrole = PathRole.Mandatory;
                }
                else
                {
                    room.pathrole = PathRole.Sidetrack;
                }
            }

            return longestPath;
        }

        private void ColorRoomDungeonSolutionPath(RoomTemplateDungeonPath path)
        {
            if (placedRooms.Count < 2)
            {
                return;
            }

            //color rooms
            foreach (Room room in placedRooms)
            {
                //Debug: different color for each room
                //ColorRoomOnMap(room, new Color(Random.Range(0F,1F),Random.Range(0F,1F),Random.Range(0F,1F),1F), 0.5F);continue;

                if (room == path.rooms[0])
                {
                    //Entrance
                    ColorRoomOnMap(room, Color.cyan, 0.3F);
                }
                else if (room == path.rooms[path.rooms.Count - 1])
                {
                    //Exit
                    ColorRoomOnMap(room, Color.red, 0.3F);
                }
                else if (path.rooms.Contains(room))
                {
                    //Main Path
                    ColorRoomOnMap(room, Color.blue, 0.3F);
                }
                else
                {
                    //Sidetrack
                    ColorRoomOnMap(room, Color.black, 0.3F);
                }
            }

            /*		
        //print path	
        Door prev = null;
        foreach(Door door in longestPath.doors){
            if(prev != null){				
                PrintLineOnMap(prev.owner.x +prev.x, prev.owner.y +prev.y, door.owner.x +door.x, door.owner.y +door.y, Color.green, 0.25F);
            }
            prev = door;
        }
        */

            mapTexture.Apply();
        }

        //create Edge to other doors in the same room
        private void CreateDoorEdgesWithinRoom(Room room)
        {
            foreach (Door door in room.doors)
            {
                foreach (Door door2 in room.doors)
                {
                    if (door != door2)
                    {
                        //reference compare
                        //using a 8 direction heuristic, diagonal counts 14, cardinal 10
                        int deltaX = Mathf.Abs(door.x - door2.x);
                        int deltaY = Mathf.Abs(door.y - door2.y);
                        int dist = deltaX > deltaY
                            ? ((deltaX - deltaY) * 10 + deltaY * 14)
                            : ((deltaY - deltaX) * 10 + deltaX * 14); //x greater? go x-y straight and y diagonal

                        //more exact floating point calculation
                        //int dist = (int)(Vector2.Distance(new Vector2((float)door.x,(float)door.y), new Vector2((float)door2.x,(float)door2.y))*10F);

                        door.connections.Add(new Edge(door2, dist)); //print (dist);					
                        //PrintLineOnMap(door.owner.x +door.x, door.owner.y +door.y, door2.owner.x +door2.x, door2.owner.y +door2.y, Color.blue, 0.3F);
                    }
                }
            }
        }

        //find path between two rooms, Nodes are Doors
        public RoomTemplateDungeonPath FindPath(Room startRoom, Room endRoom)
        {
            //Dijkstra

            //prepare
            foreach (Room room in placedRooms)
            {
                foreach (Door door in room.doors)
                {
                    door.visited = false;
                    door.inList = false;
                    door.distance = 10000; //integer distance
                    door.prev = null;
                }
            }

            //every room MUST have doors, except if only one is placed, so check to be sure
            if (startRoom.doors.Count == 0)
            {
                return new RoomTemplateDungeonPath();
            } //empty list

            //start at a random door from starting room, try to find any door of end room
            Door start = startRoom.doors[0];

            //start.visited = true;
            start.distance = 0;

            //copy list

            Door current = start;
            int alt; // = current.distance;//0

            List<Door> searchList = new List<Door>();
            searchList.Add(start);

            int failsave = 1000;

            while (searchList.Count > 0)
            {
                failsave--;
                if (failsave == 0)
                {
                    print("door graph error");
                    return new RoomTemplateDungeonPath();
                }

                current = searchList.Min();
                if (current.owner == endRoom)
                {
                    break;
                }

                searchList.Remove(current);

                foreach (Edge e in current.connections)
                {
                    //if(e.node2 == current){print ("edge references itself"); break;}

                    //add nodes to List
                    if (!e.node2.inList)
                    {
                        searchList.Add(e.node2);
                        e.node2.inList = true;
                    }

                    //Update Distances if alternative is better
                    alt = current.distance + e.length;
                    if (alt < e.node2.distance)
                    {
                        //if a shorter path to this noe is discovered, prev is current
                        e.node2.distance = alt;
                        e.node2.prev = current;
                    }
                }
            }

            //build path
            RoomTemplateDungeonPath path = new RoomTemplateDungeonPath();
            path.length = current.distance; //length is known already, its te distance of the last point

            while (current.prev != null)
            {
                path.doors.Add(current);
                if (current.owner != current.prev.owner)
                {
                    path.rooms.Add(current.owner);
                } //if room changes in next step, add it

                current = current.prev;

                if (path.doors.Count > 1000)
                {
                    print("looping error, do you have an edge with length 0?");
                    break;
                }
            }

            path.doors.Add(current);
            path.rooms.Add(current.owner);
            path.doors.Reverse();
            path.rooms.Reverse();

            return path;
        }

        //Random Template generation... :

        private List<RoomTemplate> GenerateRoomTemplates()
        {
            List<RoomTemplate> newTemplates = new List<RoomTemplate>();


            for (int i = 0; i < randomTemplateCount; i++)
            {
                //smallest room = 6 (4x4 for door)
                newTemplates.Add(GenerateRandomRoomTemplate());
                newTemplates[newTemplates.Count - 1].name = "Template " + i.ToString("000");
            }

            return newTemplates;
        }

        private RoomTemplate GenerateRandomRoomTemplate()
        {
            int xSize = Random.Range(minRoomSize, maxRoomSize + 1);
            int ySize = Random.Range(minRoomSize, maxRoomSize + 1);

            RoomTemplate roomTemplate = new RoomTemplate();

            //create new room
            roomTemplate.tiles = CreateUnborderedRectRoom(xSize, ySize);

            //chance to overlay a second room for more diversity (only in positive direction, since it doesn't matter which rect came first when we only have 2)
            for (int i = 0; i < overlayCount; i++)
            {
                if (Random.Range(0, 101) < overlayChance)
                {
                    roomTemplate.tiles = OverlayNewRoom(roomTemplate.tiles);
                }
            }


            //print (roomTemplate.height);

            //chance to ovelay offset / rotate a copy of itself
            if (Random.Range(0F, 1F) < 0.5F)
            {
                //50%
                MirrorTiles(roomTemplate.tiles);
            }

            //chance to ovelay offset / rotate a copy of itself
            if (Random.Range(0F, 1F) < 0.3F)
            {
                //30%
            }


            //add Border		
            CreateBorders(roomTemplate.tiles);

            if (Random.Range(0, 101) < wrinkledRoomProbability)
            {
                IngrowRoomBorderTiles(roomTemplate.tiles);
            }

            FillRoomCavities(
                roomTemplate); //if room is has seperated floor tiles, remove all but the biggest connected bunch

            CutRoom(roomTemplate); //cut away all tiles that have not a single floor as neighbor (in 8 directions)

            GeneratePotentialDoors(roomTemplate); //Mark walls that would allow transitions into other rooms

            return roomTemplate;
        }

        private Tile[,] CreateBorderedRectRoom(int xSize, int ySize)
        {
            Tile[,] tiles = new Tile[xSize, ySize];

            //border
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (x == 0 || y == 0 || x == xSize - 1 || y == ySize - 1)
                    {
                        tiles[x, y] = Tile.Wall;
                    }
                }
            }

            //insides
            for (int x = 1; x < xSize - 1; x++)
            {
                for (int y = 1; y < ySize - 1; y++)
                {
                    tiles[x, y] = Tile.Floor;
                }
            }

            return tiles;
        }

        private Tile[,] CreateUnborderedRectRoom(int xSize, int ySize)
        {
            Tile[,] tiles = new Tile[xSize, ySize];
            //insides
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    tiles[x, y] = Tile.Floor;
                }
            }

            return tiles;
        }

        private void MirrorTiles(Tile[,] roomTiles)
        {
            int xSize = roomTiles.GetLength(0);
            int ySize = roomTiles.GetLength(1);

            for (int x = 0; x < xSize; x++)
            {
                if (x < xSize / 2)
                {
                    for (int y = 0; y < ySize; y++)
                    {
                        //swap
                        Tile temp = roomTiles[x, y];
                        int xM = xSize - x - 1; //opposite cell
                        roomTiles[x, y] = roomTiles[xM, y];
                        roomTiles[xM, y] = temp;
                    }
                }
            }
        }

        private void TransposeTiles(Tile[,] roomTiles)
        {
        }

        private void RotateTiles(Tile[,] roomTiles)
        {
        }

        private void IngrowRoomBorderTiles(Tile[,] roomTiles)
        {
            //D array is passed by ref

            int xSize = roomTiles.GetLength(0);
            int ySize = roomTiles.GetLength(1);

            //chance to add wall if wall is adjacent already
            for (int x = 1; x < xSize - 1; x++)
            {
                for (int y = 1; y < ySize - 1; y++)
                {
                    if (roomTiles[x, y] == Tile.Floor)
                    {
                        int adj = adjacentWalls(roomTiles, x, y);
                        if (adj > 0 && Random.Range(0F, 0.45F) + adj * 0.1F > 0.35F)
                        {
                            roomTiles[x, y] = Tile.Wall;
                        }
                    }
                }
            }
        }

        private Tile[,] OverlayNewRoom(Tile[,] oldTiles)
        {
            int xSize = oldTiles.GetLength(0);
            int ySize = oldTiles.GetLength(1);

            //random offset within old room, -1 because border on border would create unpassabel walls
            int offX = Random.Range(0, xSize - 1);
            int offY = Random.Range(0, ySize - 1);

            //new room size
            int xSizeOverlay = Random.Range(minRoomSize, maxRoomSize + 1);
            int ySizeOverlay = Random.Range(minRoomSize, maxRoomSize + 1);

            //ovelay room tiles
            Tile[,] overlay = CreateUnborderedRectRoom(xSizeOverlay, ySizeOverlay);

            //new Array containing both rooms, it may happen that new room is completely within old, so use Max
            int newXSize = Mathf.Max(offX + xSizeOverlay, xSize);
            int newYSize = Mathf.Max(offY + ySizeOverlay, ySize);
            Tile[,] newTiles = new Tile[newXSize, newYSize];

            //copy both into it
            for (int x = 0; x < newXSize; x++)
            {
                for (int y = 0; y < newYSize; y++)
                {
                    //copy old room
                    if (x < xSize && y < ySize)
                    {
                        newTiles[x, y] = oldTiles[x, y];
                    }

                    //overlay new
                    if (x >= offX && y >= offY)
                    {
                        // if on or past the origin of overlay
                        if (x < offX + xSizeOverlay && y < offY + ySizeOverlay)
                        {
                            // if room is completely contained we have to check if the overlay array isnt out of bounds
                            Tile oldTile = newTiles[x, y];
                            Tile overTile = overlay[x - offX, y - offY];

                            if (oldTile == Tile.None)
                            {
                                //if new is overlaying None: replace
                                newTiles[x, y] = overTile;
                            }
                            else if (oldTile == Tile.Floor || overTile == Tile.Floor)
                            {
                                //floor always wins, overrwriting old walls too if walls already exist on starting room
                                newTiles[x, y] = Tile.Floor;
                            }
                        }
                    }
                }
            }

            // print (oldTiles.GetLength(0)+"|"+offX+"|"+xSize+"|"+xSizeOverlay+"|"+newXSize);
            return newTiles;
        }

        //don't give border coordinates
        private int adjacentWalls(Tile[,] tiles, int x, int y)
        {
            int adj = 0;
            if (tiles[x - 1, y + 0] == Tile.Wall)
            {
                adj++;
            }

            if (tiles[x + 0, y + 1] == Tile.Wall)
            {
                adj++;
            }

            if (tiles[x + 1, y + 0] == Tile.Wall)
            {
                adj++;
            }

            if (tiles[x + 0, y - 1] == Tile.Wall)
            {
                adj++;
            }

            return adj;
        }

        private void FillRoomCavities(RoomTemplate roomTemplate)
        {
            //i wasn't able to get  4-way Connected-component labeling running so i just use this recursive method...

            int xSize = roomTemplate.tiles.GetLength(0);
            int ySize = roomTemplate.tiles.GetLength(1);

            int[,] roomLabels = new int[xSize, ySize]; //defaults to 0

            int regionLabel = 0;
            int largestRegionLabel = -1;
            int largestSize = -1;


            for (int x = 1; x < xSize - 1; x++)
            {
                for (int y = 1; y < ySize - 1; y++)
                {
                    if (roomLabels[x, y] == 0 && roomTemplate.tiles[x, y] == Tile.Floor)
                    {
                        regionLabel++;
                        int size = FillNeighbors(ref roomTemplate.tiles, regionLabel, x, y, ref roomLabels, 0);
                        if (size > largestSize)
                        {
                            largestSize = size;
                            largestRegionLabel = regionLabel;
                        }
                    }
                }
            }

            if (regionLabel == 1)
            {
                return;
            } //only one region, no need to fill

            for (int x = 1; x < xSize - 1; x++)
            {
                for (int y = 1; y < ySize - 1; y++)
                {
                    if (roomLabels[x, y] != largestRegionLabel && roomLabels[x, y] != 0)
                    {
                        roomTemplate.tiles[x, y] = Tile.Wall;
                    }
                }
            }
        }

        private int FillNeighbors(ref Tile[,] tiles, int label, int x, int y, ref int[,] roomLabels, int depth)
        {
            //visited
            roomLabels[x, y] = label;
            int hits = 1;
            depth++;
            if (depth == 3000)
            {
                print("depth error");
                return 10000;
            }

            //edge is wall, so no need to check for out of bounds
            if (roomLabels[x + 0, y + 1] == 0 && tiles[x + 0, y + 1] == Tile.Floor)
            {
                hits += FillNeighbors(ref tiles, label, x + 0, y + 1, ref roomLabels, depth);
            }

            if (roomLabels[x + 1, y + 0] == 0 && tiles[x + 1, y + 0] == Tile.Floor)
            {
                hits += FillNeighbors(ref tiles, label, x + 1, y + 0, ref roomLabels, depth);
            }

            if (roomLabels[x + 0, y - 1] == 0 && tiles[x + 0, y - 1] == Tile.Floor)
            {
                hits += FillNeighbors(ref tiles, label, x + 0, y - 1, ref roomLabels, depth);
            }

            if (roomLabels[x - 1, y + 0] == 0 && tiles[x - 1, y + 0] == Tile.Floor)
            {
                hits += FillNeighbors(ref tiles, label, x - 1, y + 0, ref roomLabels, depth);
            }

            return hits;
        }

        //Cut away adges of a room by testing tiles in all directions if wall is obsolete
        private void CutRoom(RoomTemplate roomTemplate)
        {
            //return;

            int xSize = roomTemplate.tiles.GetLength(0);
            int ySize = roomTemplate.tiles.GetLength(1);

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    bool obsolete = true;

                    if (roomTemplate.tiles[x, y] == Tile.Wall)
                    {
                        bool ignoreN = y == ySize - 1;
                        bool ignoreE = x == xSize - 1;
                        bool ignoreS = y == 0;
                        bool ignoreW = x == 0;

                        //if one adjacent block is empty, the block isn't obsolete
                        obsolete = obsolete && (ignoreN || (roomTemplate.tiles[x + 0, y + 1] == Tile.Wall ||
                                                            roomTemplate.tiles[x + 0, y + 1] == Tile.None)); //N
                        obsolete = obsolete && (ignoreN || ignoreE ||
                                                (roomTemplate.tiles[x + 1, y + 1] == Tile.Wall ||
                                                 roomTemplate.tiles[x + 1, y + 1] == Tile.None)); //NE
                        obsolete = obsolete && (ignoreE || (roomTemplate.tiles[x + 1, y + 0] == Tile.Wall ||
                                                            roomTemplate.tiles[x + 1, y + 0] == Tile.None)); //E
                        obsolete = obsolete && (ignoreS || ignoreE ||
                                                (roomTemplate.tiles[x + 1, y - 1] == Tile.Wall ||
                                                 roomTemplate.tiles[x + 1, y - 1] == Tile.None)); //SE
                        obsolete = obsolete && (ignoreS || (roomTemplate.tiles[x + 0, y - 1] == Tile.Wall ||
                                                            roomTemplate.tiles[x + 0, y - 1] == Tile.None)); //S
                        obsolete = obsolete && (ignoreS || ignoreW ||
                                                (roomTemplate.tiles[x - 1, y - 1] == Tile.Wall ||
                                                 roomTemplate.tiles[x - 1, y - 1] == Tile.None)); //SW
                        obsolete = obsolete && (ignoreW || (roomTemplate.tiles[x - 1, y + 0] == Tile.Wall ||
                                                            roomTemplate.tiles[x - 1, y + 0] == Tile.None)); //W
                        obsolete = obsolete && (ignoreN || ignoreW ||
                                                (roomTemplate.tiles[x - 1, y + 1] == Tile.Wall ||
                                                 roomTemplate.tiles[x - 1, y + 1] == Tile.None)); //NW

                        if (obsolete)
                        {
                            roomTemplate.tiles[x, y] = Tile.None;
                        }
                    }
                }
            }

            //Remove eventual cavities we have created by only allowing the biggest cavity to exist
        }

        //Convert FloorTiles to make a Border(Wall) on plain Floor-rooms, if a Floor Tile has Tile.None AND Tile.Floor as neighbor (8 dir) it is converted to a border
        private void CreateBorders(Tile[,] tiles)
        {
            int xSize = tiles.GetLength(0);
            int ySize = tiles.GetLength(1);

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    bool noneAdj = false; //at least 1 None adjacent
                    bool flooAdj = false; //at least 1 Floor adjacent		

                    if (tiles[x, y] == Tile.Floor)
                    {
                        bool ignoreN = y == ySize - 1;
                        bool ignoreE = x == xSize - 1;
                        bool ignoreS = y == 0;
                        bool ignoreW = x == 0;

                        //if one adjacent block is None, condition is met
                        noneAdj = noneAdj || (ignoreN || (tiles[x + 0, y + 1] == Tile.None)); //N
                        noneAdj = noneAdj || (ignoreN || ignoreE || (tiles[x + 1, y + 1] == Tile.None)); //NE
                        noneAdj = noneAdj || (ignoreE || (tiles[x + 1, y + 0] == Tile.None)); //E
                        noneAdj = noneAdj || (ignoreS || ignoreE || (tiles[x + 1, y - 1] == Tile.None)); //SE
                        noneAdj = noneAdj || (ignoreS || (tiles[x + 0, y - 1] == Tile.None)); //S
                        noneAdj = noneAdj || (ignoreS || ignoreW || (tiles[x - 1, y - 1] == Tile.None)); //SW
                        noneAdj = noneAdj || (ignoreW || (tiles[x - 1, y + 0] == Tile.None)); //W
                        noneAdj = noneAdj || (ignoreN || ignoreW || (tiles[x - 1, y + 1] == Tile.None)); //NW

                        //if one adjacent block is Floor, condition is met
                        flooAdj = flooAdj || (!ignoreN && (tiles[x + 0, y + 1] == Tile.Floor)); //N
                        flooAdj = flooAdj || (!ignoreN && !ignoreE && (tiles[x + 1, y + 1] == Tile.Floor)); //NE
                        flooAdj = flooAdj || (!ignoreE && (tiles[x + 1, y + 0] == Tile.Floor)); //E
                        flooAdj = flooAdj || (!ignoreS && !ignoreE && (tiles[x + 1, y - 1] == Tile.Floor)); //SE
                        flooAdj = flooAdj || (!ignoreS && (tiles[x + 0, y - 1] == Tile.Floor)); //S
                        flooAdj = flooAdj || (!ignoreS && !ignoreW && (tiles[x - 1, y - 1] == Tile.Floor)); //SW
                        flooAdj = flooAdj || (!ignoreW && (tiles[x - 1, y + 0] == Tile.Floor)); //W
                        flooAdj = flooAdj || (!ignoreN && !ignoreW && (tiles[x - 1, y + 1] == Tile.Floor)); //NW

                        if (noneAdj && flooAdj)
                        {
                            tiles[x, y] = Tile.Wall;
                        }
                    }
                }
            }
        }

        private void GeneratePotentialDoors(RoomTemplate roomTemplate)
        {
            //for doorlength 1 and 2, we need at least 3 and 4 adjacent tiles in line with free entrance tiles (Floor or roomend)
            //room consists out of Tiles.Floor/Wall/None as of now

            int xSize = roomTemplate.tiles.GetLength(0);
            int ySize = roomTemplate.tiles.GetLength(1);

            Tile[,] roomTemplateTilesCopy = new Tile[xSize, ySize];
            System.Array.Copy(roomTemplate.tiles, roomTemplateTilesCopy, xSize * ySize);


            //scan for doors in horizontal(EW) and vertical(NS) direction
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (roomTemplate.tiles[x, y] == Tile.Wall)
                    {
                        //on the edges og the room tiles are considered open
                        bool noneN = y == ySize - 1;
                        bool noneS = y == 0;
                        bool noneE = x == xSize - 1;
                        bool noneW = x == 0;

                        //if one adjacent block is empty, the block isn't obsolete
                        Tile nTile = noneN ? Tile.None : roomTemplate.tiles[x + 0, y + 1]; //N
                        Tile sTile = noneS ? Tile.None : roomTemplate.tiles[x + 0, y - 1]; //S
                        Tile eTile = noneE ? Tile.None : roomTemplate.tiles[x + 1, y + 0]; //E
                        Tile wTile = noneW ? Tile.None : roomTemplate.tiles[x - 1, y + 0]; //W

                        //Doordirections, N-door means leaving room in N direction

                        if (nTile == Tile.None && sTile == Tile.Floor && eTile == Tile.Wall && wTile == Tile.Wall)
                        {
                            roomTemplateTilesCopy[x, y] = Tile.DoorNorth;
                        } //door North
                        else if (nTile == Tile.Floor && sTile == Tile.None && eTile == Tile.Wall && wTile == Tile.Wall)
                        {
                            roomTemplateTilesCopy[x, y] = Tile.DoorSouth;
                        } //door South
                        else if (nTile == Tile.Wall && sTile == Tile.Wall && eTile == Tile.None && wTile == Tile.Floor)
                        {
                            roomTemplateTilesCopy[x, y] = Tile.DoorEast;
                        } //door East
                        else if (nTile == Tile.Wall && sTile == Tile.Wall && eTile == Tile.Floor && wTile == Tile.None)
                        {
                            roomTemplateTilesCopy[x, y] = Tile.DoorWest;
                        } //door West
                    }
                }
            }

            //DEBUG: write copy back into Room to see how tiles got assigned
            //System.Array.Copy( roomTilesCopy, room.tiles, xSize*ySize );


            roomTemplate.potentialDoors = new List<Door>();

            //Add Doors to room		
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (roomTemplateTilesCopy[x, y] != Tile.None && roomTemplateTilesCopy[x, y] != Tile.Wall)
                    {
                        //is door?

                        bool noneN = y == ySize - 1;
                        //bool noneS = y == 0;
                        bool noneE = x == xSize - 1;
                        //bool noneW = x == 0;

                        //if one adjacent block is empty, the block isn't obsolete
                        Tile nTile = noneN ? Tile.None : roomTemplateTilesCopy[x + 0, y + 1]; //N
                        //Tile sTile	= noneS ?	Tile.None:	roomTemplateTilesCopy[x+0,y-1];	//S
                        Tile eTile = noneE ? Tile.None : roomTemplateTilesCopy[x + 1, y + 0]; //E
                        //Tile wTile	= noneW ?	Tile.None:	roomTemplateTilesCopy[x-1,y+0];	//W

                        //Doordirections, N-door means leaving room in N direction

                        if (roomTemplateTilesCopy[x, y] == Tile.DoorNorth ||
                            roomTemplateTilesCopy[x, y] == Tile.DoorSouth)
                        {
                            //door North

                            DoorDirection dir = roomTemplateTilesCopy[x, y] == Tile.DoorNorth
                                ? DoorDirection.North
                                : DoorDirection.South;


                            //check if we have a door of length 2 by checking if right tile is same door type
                            if (doors2x2Allowed && eTile == roomTemplateTilesCopy[x, y])
                            {
                                roomTemplate.potentialDoors.Add(new Door(dir, 2, x, y));
                                //roomTemplateTilesCopy[x+0,y+1]	= Tile.Wall;	//remove DoorMark
                            }
                            else
                            {
                                roomTemplate.potentialDoors.Add(new Door(dir, 1, x, y));
                            }

                            roomTemplateTilesCopy[x, y] = Tile.Wall; //remove DoorMark
                        }
                        else if (roomTemplateTilesCopy[x, y] == Tile.DoorEast ||
                                 roomTemplateTilesCopy[x, y] == Tile.DoorWest)
                        {
                            //door South

                            DoorDirection dir = roomTemplateTilesCopy[x, y] == Tile.DoorEast
                                ? DoorDirection.East
                                : DoorDirection.West;

                            //check if we have a door of length 2 by checking if top tile is same door type
                            if (doors2x2Allowed && nTile == roomTemplateTilesCopy[x, y])
                            {
                                roomTemplate.potentialDoors.Add(new Door(dir, 2, x, y));
                                //roomTemplateTilesCopy[x+0,y+1]	= Tile.Wall;	//remove DoorMark
                            }
                            else
                            {
                                roomTemplate.potentialDoors.Add(new Door(dir, 1, x, y));
                            }

                            roomTemplateTilesCopy[x, y] = Tile.Wall; //remove DoorMark								
                        }
                    }
                }
            }
        }

        #endregion


        #region RoomTemplateReader

        //reads rooms from bitmap file

        public Texture2D templateSheet;

        private Color32 keyVoid = new Color32(000, 000, 000, 000); //transparent
        private Color32 keyHole = new Color32(000, 000, 255, 255); //transparent
        private Color32 keyWall = new Color32(000, 000, 000, 255); //black
        private Color32 keyFloor = new Color32(255, 255, 255, 255); //white
        private Color32 keyDoor = new Color32(000, 255, 000, 255); //green
        private Color32 keySpawn = new Color32(255, 000, 000, 255); //red

        private bool[,] visited;
        private int texWidth;
        private int texHeight;

        public List<RoomTemplate> ParseTexture(int texIndex)
        {
            List<RoomTemplate> roomTemplates = new List<RoomTemplate>();

            Texture2D tex = templateSheet;
            //if(texIndex == 0){	tex = templateSheet;	}

            Color32[] data = tex.GetPixels32();
            int dataLength = data.Length;
            Color32[,] pixels = new Color32[tex.width, tex.height];
            //set up varaibles if not
            texWidth = pixels.GetLength(0);
            texHeight = pixels.GetLength(1);

            int x0 = 0;
            int y0 = 0;
            for (int i = 0; i < dataLength; i++)
            {
                pixels[x0, y0] = data[i];
                x0 = x0 + 1;
                if (x0 == texWidth)
                {
                    x0 = 0;
                    y0++;
                }
            }

            //parse texture for rooms, starts at lower left since levels and rooms are defined with lower left as (0/0)

            visited = new bool[texWidth, texHeight]; //default false

            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    if (!visited[x, y] && !pixels[x, y].Equals(keyVoid))
                    {
                        //	print ("roomstart found at: "+x+"/"+y);
                        roomTemplates.Add(ReadRoom(ref pixels, x, y));
                    }
                }
            }

            return roomTemplates;
        }

        private RoomTemplate ReadRoom(ref Color32[,] pixels, int x, int y)
        {
            HashSet<Coordinate> dynGrid = new HashSet<Coordinate>();
            RecursiveRoomExtract(ref pixels, dynGrid, x, y);

            RoomTemplate newTemplate = new RoomTemplate(CreateTileArray(dynGrid), new List<Door>());

            newTemplate.name = x + "/" + y;
            newTemplate.potentialDoors = GenerateDoorList(newTemplate.tiles);
            newTemplate.spawners = GenerateSpawnerList(newTemplate.tiles);

            //	print ("spawners:"+newTemplate.spawners.Count);

            return newTemplate;
        }

        private List<Door> GenerateDoorList(Tile[,] tiles)
        {
            List<Door> potentialDoors = new List<Door>();


            int xSize = tiles.GetLength(0);
            int ySize = tiles.GetLength(1);

            //Add Doors to room		
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    //parsing from bottom left to top right
                    //Doordirections: DoorNorth means leaving direction	
                    switch (tiles[x, y])
                    {
                        //	▼ only create door for lower tile, this function only uses doors of lenght 2						//AfterDoor has been added to list, set Tiles to wall, doorLink algorithm will carve them out again
                        case Tile.DoorNorth:
                            if (tiles[x + 1, y] == Tile.DoorNorth)
                            {
                                potentialDoors.Add(new Door(DoorDirection.North, 2, x, y));
                            }

                            tiles[x, y] = Tile.Wall;
                            break;
                        case Tile.DoorSouth:
                            if (tiles[x + 1, y] == Tile.DoorSouth)
                            {
                                potentialDoors.Add(new Door(DoorDirection.South, 2, x, y));
                            }

                            tiles[x, y] = Tile.Wall;
                            break;
                        case Tile.DoorEast:
                            if (tiles[x, y + 1] == Tile.DoorEast)
                            {
                                potentialDoors.Add(new Door(DoorDirection.East, 2, x, y));
                            }

                            tiles[x, y] = Tile.Wall;
                            break;
                        case Tile.DoorWest:
                            if (tiles[x, y + 1] == Tile.DoorWest)
                            {
                                potentialDoors.Add(new Door(DoorDirection.West, 2, x, y));
                            }

                            tiles[x, y] = Tile.Wall;
                            break;
                    }
                }
            }

            //	print (potentialDoors.Count);
            return potentialDoors;
        }

        private List<IntVector2> GenerateSpawnerList(Tile[,] tiles)
        {
            List<IntVector2> spawners = new List<IntVector2>();

            int xSize = tiles.GetLength(0);
            int ySize = tiles.GetLength(1);

            //Add Spawnpositions to room		
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    switch (tiles[x, y])
                    {
                        case Tile.Spawner:
                            spawners.Add(new IntVector2(x, y));
                            tiles[x, y] = Tile.Floor;
                            break;
                    }
                }
            }

            //	print ("spawners:"+spawners.Count);
            return spawners;
        }


        private void RecursiveRoomExtract(ref Color32[,] pixels, HashSet<Coordinate> dynGrid, int x, int y)
        {
            //inside texture bounds?
            if (x < 0 || x >= texWidth)
            {
                return;
            }

            if (y < 0 || y >= texHeight)
            {
                return;
            }

            //already visited?
            if (visited[x, y])
            {
                return;
            }

            visited[x, y] = true;

            Color32 col = pixels[x, y];

            if (col.Equals(keyVoid))
            {
                return;
            } //outside of current room
            else if (col.Equals(keyHole))
            {
                dynGrid.Add(new Coordinate(x, y, Tile.Hole));
            } //Hole
            else if (col.Equals(keyWall))
            {
                dynGrid.Add(new Coordinate(x, y, Tile.Wall));
            } //Wall
            else if (col.Equals(keyFloor))
            {
                dynGrid.Add(new Coordinate(x, y, Tile.Floor));
            } //Floor
            else if (col.Equals(keySpawn))
            {
                dynGrid.Add(new Coordinate(x, y, Tile.Spawner));
            } //Floor, but with Spawner
            else if (col.Equals(keyDoor))
            {
                dynGrid.Add(new Coordinate(x, y, CheckDoorDirection(ref pixels, x, y)));
            }

            RecursiveRoomExtract(ref pixels, dynGrid, x + 1, y + 0);
            RecursiveRoomExtract(ref pixels, dynGrid, x + 0, y + 1);
            RecursiveRoomExtract(ref pixels, dynGrid, x - 1, y - 0);
            RecursiveRoomExtract(ref pixels, dynGrid, x - 0, y - 1);
        }

        private Tile CheckDoorDirection(ref Color32[,] pixels, int x, int y)
        {
            //search for side that has void, Doordirections: DoorNorth means leaving room while by going North
            if (x + 1 < texWidth && pixels[x + 1, y + 0].Equals(keyVoid))
            {
                return Tile.DoorEast;
            } //check right
            else if (y + 1 < texHeight && pixels[x + 0, y + 1].Equals(keyVoid))
            {
                return Tile.DoorNorth;
            } //check up
            else if (x - 1 >= 0 && pixels[x - 1, y - 0].Equals(keyVoid))
            {
                return Tile.DoorWest;
            } //check left
            else if (y - 1 >= 0 && pixels[x - 0, y - 1].Equals(keyVoid))
            {
                return Tile.DoorSouth;
            } //check down

            //error
            print("error, template contains inner door");

            return Tile.Door;
        }

        private Tile[,] CreateTileArray(HashSet<Coordinate> dynGrid)
        {
            if (dynGrid.Count == 0)
            {
                print("dynamic grid is empty, maybe room template texture is imported with compression?");
                return null;
            }

            int minX = texWidth; //can only get smaller
            int minY = texHeight; //can only get smaller
            int maxX = 0; //can only get larger
            int maxY = 0; //can only get larger

            //find room extents
            foreach (Coordinate c in dynGrid)
            {
                if (c.x < minX)
                {
                    minX = c.x;
                }

                if (c.y < minY)
                {
                    minY = c.y;
                }

                if (c.x > maxX)
                {
                    maxX = c.x;
                }

                if (c.y > maxY)
                {
                    maxY = c.y;
                }
            }

            //	print ("Tiles:"+dynGrid.Count);		
            //	print ("X:"+minX+"-"+maxX);
            //	print ("Y:"+minY+"-"+maxY);

            int sizeX = maxX - minX + 1;
            int sizeY = maxY - minY + 1;

            //	print ("Size:"+sizeX+"/"+sizeY);

            //Init Array with empty Tiles
            Tile[,] roomTiles = new Tile[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    roomTiles[x, y] = Tile.None;
                }
            }

            foreach (Coordinate c in dynGrid)
            {
                roomTiles[c.x - minX, c.y - minY] = c.tileType;
            }

            return roomTiles;
        }

        //Dynamic Coordinate Grid http://stackoverflow.com/questions/1939319/defining-two-dimensional-dynamic-array
        public class Coordinate /* : IEquatable<Coordinate>*/
        {
            public Coordinate(int x, int y, Tile tileType)
            {
                this.x = x;
                this.y = y;
                this.tileType = tileType;
            }

            public int x; //{ get; }
            public int y; //{ get; }

            public Tile tileType;
            // override Equals and GetHashcode...
        }

        #endregion

        #region HAUBERK (Rooms + Mazes)

        //Gathered from http://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
        //converted to C# for use in Unity by Marrt

        /// The random dungeon generator.
        ///
        /// Starting with a stage of solid walls, it works like so:
        ///
        /// 1. Place a number of randomly sized and positioned rooms. If a room
        ///	overlaps an existing room, it is discarded. Any remaining rooms are
        ///	carved out.
        /// 2. Any remaining solid areas are filled in with mazes. The maze generator
        ///	will grow and fill in even odd-shaped areas, but will not touch any
        ///	rooms.
        /// 3. The result of the previous two steps is a series of unconnected rooms
        ///	and mazes. We walk the stage and find every tile that can be a
        ///	"connector". This is a solid tile that is adjacent to two unconnected
        ///	regions.
        /// 4. We randomly choose connectors and open them or place a door there until
        ///	all of the unconnected regions have been joined. There is also a slight
        ///	chance to carve a connector between two already-joined regions, so that
        ///	the dungeon isn't single connected.
        /// 5. The mazes will have a lot of dead ends. Finally, we remove those by
        ///	repeatedly filling in any open tile that's closed on three sides. When
        ///	this is done, every corridor in a maze actually leads somewhere.
        ///
        /// The end result of this is a multiply-connected dungeon with rooms and lots
        /// of winding corridors.

        //Hauberk variables
        //private	Rect bounds					= new Rect(0,0,0,0);
        [Header("	Hauberk Dungeon")] public int numRoomTries = 300; //Room placement tries

        public int
            extraConnectorChance =
                5; //The inverse chance of adding a connector between two regions that have already been joined for more interconnection between regions

        public int roomExtraSize = 4; //Increasing this allows rooms to be larger.
        public int windingPercent = 20; //chance a maze will make a turn which will make it more winding

        public bool
            tryRoomsFirst =
                false; //try to make room-to-room connections before making corridor-to-room connections (corridor-to-corridor are impossible)

        public bool streamLine = true; //streamline corridors between branchpoints and doors
        private List<Rect> _rooms; //list of placed rooms

        private int
            _currentRegion =
                -1; // The index of the current region (=connected carved area) being carved, -1 = default, wall

        public List<IntVector2> cardinal; //original implementation of Hauberk used Direction.CARDINAL	

        /// <summary>Generate a room and maze dungeon, http://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/ </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="numRoomTries">Room placement tries</param>
        /// <param name="extraConnectorChance">The inverse chance of adding a connector between two regions that have already been joined for more interconnection between regions</param>
        /// <param name="roomExtraSize">Increasing this allows rooms to be larger</param>
        /// <param name="windingPercent">chance a maze will make a turn which will make it more winding</param>
        /// <param name="tryRoomsFirst">chance a maze will make a turn which will make it more winding</param>
        /// <param name="seed">use random seed for this generation, ignores _randomSeedOn</param>
        /// 
        public Dungeon GenerateHauberkDungeon(int? width = null,
            int? height = null,
            int? numRoomTries = null,
            int? extraConnectorChance = null,
            int? roomExtraSize = null,
            int? windingPercent = null,
            bool? tryRoomsFirst = null,
            bool? streamLine = null,
            int? seed = null)
        {
            PerformanceMeter.StartTimeLog("tilegrid generation:\t");

            _dungeonWidth = width ?? _dungeonWidth;
            _dungeonHeight = height ?? _dungeonHeight;

            //check size
            if (_dungeonWidth % 2 == 0 || _dungeonHeight % 2 == 0)
            {
                print("The stage must be odd-sized");
                return null;
            }

            //optional parameters to overwrite public ones set in editor
            this.numRoomTries = numRoomTries ?? this.numRoomTries;
            this.extraConnectorChance = extraConnectorChance ?? this.extraConnectorChance;
            this.roomExtraSize = roomExtraSize ?? this.roomExtraSize;
            this.windingPercent = windingPercent ?? this.windingPercent;
            this.tryRoomsFirst = tryRoomsFirst ?? this.tryRoomsFirst;
            this.streamLine = streamLine ?? this.streamLine;
            _seed = seed ?? _seed;

            if (_randomSeedOn || seed != null)
            {
                //use random seed if overlaod was used, or if it is enabled in general
                Random.seed = _seed;
            }

            //init grids
            _dungeon = new Tile[_dungeonWidth, _dungeonHeight];
            _regions = new int[_dungeonWidth, _dungeonHeight];
            _currentRegion = -1; //reset

            //print("size:"+_dungeonHeight+"|"+_dungeonWidth+"\n");

            for (var x = 0; x < _dungeonWidth; x++)
            {
                for (var y = 0; y < _dungeonHeight; y++)
                {
                    setTile(new IntVector2(x, y), Tile.Wall);
                    _regions[x, y] = _currentRegion; //-1
                }
            }

            _addRooms(); //randomly place rooms

            // Fill in all of the empty space with mazes.
            for (var x = 1; x < _dungeonWidth; x += 2)
            {
                for (var y = 1; y < _dungeonHeight; y += 2)
                {
                    var pos = new IntVector2(x, y);
                    if (getTile(pos) != Tile.Wall) continue; //ignore already carved spaces
                    _growMaze(pos);
                }
            }

            _connectRegions();
            _removeDeadEnds();


            //function Marrt added to streamline corridors
            if (this.streamLine)
            {
                _streamLineCorridors();
            }

            MultiplyDungeon(2);
            mapPxPerTile = 2F;

            PerformanceMeter.EndTimeLog();
            PerformanceMeter.AddLog("");

            var dungeon = new Dungeon(_dungeon, _regions);

            dungeon.entrance = FindFloorInDungeon(dungeon);
            dungeon.entranceSet = true;

            UpdateMap(ref _dungeon);
            //UpdateMap(ref _regions);

            return dungeon;
        }


        /// Implementation of the "growing tree" algorithm from here:
        /// http://www.astrolog.org/labyrnth/algrithm.htm.
        private void _growMaze(IntVector2 start)
        {
            List<IntVector2> cells = new List<IntVector2>();
            IntVector2 lastDir = null;


            _startRegion();
            _carve(start);

            cells.Add(start);

            while (cells.Count > 0)
            {
                IntVector2 cell = cells[cells.Count - 1]; //last element in list

                // See which adjacent cells are open.
                List<IntVector2> unmadeCells = new List<IntVector2>();

                foreach (IntVector2 dir in cardinal)
                {
                    if (_canCarve(cell, dir)) unmadeCells.Add(dir);
                }

                if (unmadeCells.Count > 0)
                {
                    // Based on how "windy" passages are, try to prefer carving in the
                    // same direction.
                    IntVector2 dir;
                    if (unmadeCells.Contains(lastDir) && Random.Range(0, 100) < windingPercent)
                    {
                        dir = lastDir; //keep previous direction
                    }
                    else
                    {
                        dir = unmadeCells[Random.Range(0, unmadeCells.Count)]; //pick new direction out of possible ones
                    }

                    _carve(cell + dir); //carve out wall between the valid cells
                    _carve(cell + dir * 2); //carve out valid cell

                    cells.Add(cell + dir * 2);
                    lastDir = dir;
                }
                else
                {
                    // No adjacent uncarved cells.
                    cells.RemoveAt(cells.Count - 1); //Remove Last element

                    // This path has ended.
                    lastDir = null;
                }
            }
        }

        /// Places rooms ignoring the existing maze corridors.
        private void _addRooms()
        {
            _rooms = new List<Rect>();

            for (var i = 0; i < numRoomTries; i++)
            {
                // Pick a random room size. The funny math here does two things:
                // - It makes sure rooms are odd-sized to line up with maze.
                // - It avoids creating rooms that are too rectangular: too tall and
                //   narrow or too wide and flat.
                // TODO: This isn't very flexible or tunable. Do something better here.

                int size = Random.Range(1, 3 + roomExtraSize) * 2 + 1; //rng.range(1, 3 + roomExtraSize) * 2 + 1;
                int rectangularity = Random.Range(0, 1 + size / 2) * 2; //rng.range(0, 1 + size ~/ 2) * 2;
                int width = size;
                int height = size;

                //print (size +"|"+ rectangularity +"|"+ width +"|"+ height);

                if (Random.Range(0, 2) == 0)
                {
                    //50% chance
                    width += rectangularity;
                }
                else
                {
                    height += rectangularity;
                }
                //print (size +"|"+ rectangularity +"|"+ width +"|"+ height);

                int x = (int) (Random.Range(1F, (_dungeonWidth - width) * 0.5F)) * 2 +
                        1; //rng.range((bounds.width - width) ~/ 2) * 2 + 1;
                int y = (int) (Random.Range(1F, (_dungeonHeight - height) * 0.5F)) * 2 +
                        1; //rng.range((bounds.height - height) ~/ 2) * 2 + 1;

                Rect room = new Rect(x, y, width, height);

                bool overlaps = false;
                foreach (var other in _rooms)
                {
                    if (room.Overlaps(other))
                    {
                        overlaps = true;
                        break; //break this foreach
                    }
                }

                if (overlaps) continue; //don't add room and retry

                //print (room);

                if (x + width > _dungeonWidth || y + height > _dungeonWidth)
                {
                    print("error, Room won't fit, dungeon to small or faulthy room generation");
                    return;
                }

                //add non-overlapping room
                _rooms.Add(room);

                _startRegion();


                /*foreach(IntVector2 pos in new Rect(x, y, width, height)) {
                _carve(pos);
            }*/

                for (int ix = x; ix < x + width; ix++)
                {
                    for (int iy = y; iy < y + height; iy++)
                    {
                        _carve(new IntVector2(ix, iy), Tile.RoomFloor);
                    }
                }
            }
        }


        //Marrt: was the hardest function for me to convert, there maybe errors
        private void _connectRegions()
        {
            // Find all of the tiles that can connect two (or more) regions.
            Dictionary<IntVector2, HashSet<int>>
                connectorRegions =
                    new Dictionary<IntVector2, HashSet<int>>(); //var connectorRegions = <Vec, Set<int>>{};

            //check each wall if it sits between 2 different regions and assign a Hashset to them
            //foreach (IntVector2 pos in bounds.inflate(-1)) {
            for (int ix = 1; ix < _dungeonWidth - 1; ix++)
            {
                for (int iy = 1; iy < _dungeonHeight - 1; iy++)
                {
                    IntVector2 pos = new IntVector2(ix, iy);

                    // Can't already be part of a region.
                    if (getTile(pos) != Tile.Wall) continue;

                    HashSet<int> regions = new HashSet<int>();

                    foreach (IntVector2 dir in cardinal)
                    {
                        IntVector2 indexer = (pos + dir);
                        var region = _regions[indexer.x, indexer.y];
                        //if (region != null) regions.Add(region);
                        if (region != -1) regions.Add(region);
                    }

                    if (regions.Count < 2) continue;

                    connectorRegions[pos] = regions; //add Hashset to current position
                }
            }

            List<IntVector2>
                connectors = connectorRegions.Keys.ToList(); //var connectors = connectorRegions.keys.toList();

            //Marrt: I think it would make for nicer dungeons if all room-to-room connections would be tried first, therefore sort List
            if (tryRoomsFirst)
            {
                //bring connectors that have two rooms attached, to front		
                connectors.OrderBy(delegate(IntVector2 con)
                {
                    int connectedRooms = 0;
                    foreach (IntVector2 dir in cardinal)
                    {
                        IntVector2 indexer = (con + dir);
                        if (_dungeon[indexer.x, indexer.y] == Tile.RoomFloor) connectedRooms++;
                    }

                    return 2 - connectedRooms;
                });
            }

            // Keep track of which regions have been merged. This maps an original
            // region index to the one it has been merged to.
            Dictionary<int, int> merged = new Dictionary<int, int>();
            HashSet<int> openRegions = new HashSet<int>();
            for (int i = 0; i <= _currentRegion; i++)
            {
                merged[i] = i;
                openRegions.Add(i);
            }

            // Keep connecting regions until we're down to one.
            while (openRegions.Count > 1)
            {
                //print (openRegions.Count+"|"+connectors.Count);
                IntVector2 connector;
                if (tryRoomsFirst)
                {
                    connector = connectors[0]; //room-to-room are ordered first in list
                }
                else
                {
                    connector = connectors[Random.Range(0, connectors.Count)]; //rng.item(connectors);
                }

                // Carve the connection.
                _addJunction(connector);

                // Merge the connected regions. We'll pick one region (arbitrarily) and
                // map all of the other regions to its index.
                //var regions = connectorRegions[connector].map((region) => merged[region]);
                var regions = connectorRegions[connector].Select((region) => merged[region]);

                int dest = regions.First();
                var sources = regions.Skip(1).ToList();

                // Merge all of the affected regions. We have to look at *all* of the
                // regions because other regions may have previously been merged with
                // some of the ones we're merging now.
                for (var i = 0; i <= _currentRegion; i++)
                {
                    if (sources.Contains(merged[i]))
                    {
                        merged[i] = dest;
                    }
                }

                // The sources are no longer in use.
                //openRegions.removeAll(sources);
                openRegions.RemoveWhere((source) => sources.Contains(source));

                // Remove any connectors that aren't needed anymore.
                connectors.RemoveAll(delegate(IntVector2 pos)
                {
                    //	connectors.removeWhere((pos) {

                    // If the connector no long spans different regions, we don't need it.
                    //var regionss = connectorRegions[pos].map((region) => merged[region]).toSet();


                    //	var regionss = connectorRegions[pos].Select((region) => merged[region]).ToHashSet();	
                    //above is OLD, needs Extension file, create Extensions.cs:
                    //	using UnityEngine;
                    //	using System.Collections;
                    //	using System.Collections.Generic;
                    //	public static class Extensions{
                    //http://stackoverflow.com/questions/3471899/how-to-convert-linq-results-to-hashset-or-hashedset
                    //	    public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source){ return new HashSet<T>(source); }
                    //	}


                    //NEW, no Extension	
                    HashSet<int> regionss =
                        new HashSet<int>(from x in (connectorRegions[pos].Select((region) => merged[region])) select x);

                    if (regionss.Count > 1) return false;

                    // This connecter isn't needed, but connect it occasionally so that the dungeon isn't singly-connected.
                    if (Random.Range(0, 100) < extraConnectorChance)
                    {
                        // Don't allow connectors right next to each other.				
                        foreach (IntVector2 dir in cardinal)
                        {
                            IntVector2 indexer = (pos + dir);
                            if (_dungeon[indexer.x, indexer.y] == Tile.Door) return true;
                        }

                        //if no connectors are adjacent, add additional connector
                        _addJunction(pos);
                    }

                    return true;
                });
            }
        }

        private void _addJunction(IntVector2 pos)
        {
            //open / closedness of a door can be determined in a later manipulations, so i removed it
            /*if (Random.Range(0,4)==0) {	//25%chance
            setTile(pos, Random.Range(0,3)==0 ? Tile.OpenDoor : Tile.Floor);
        } else {
            setTile(pos, Tile.ClosedDoor);
        }*/

            setTile(pos, Tile.Door);
        }


        private void _removeDeadEnds()
        {
            bool done = false;

            while (!done)
            {
                done = true;

                //foreach (IntVector2 pos in bounds.inflate(-1)) {
                for (int ix = 1; ix < _dungeonWidth - 1; ix++)
                {
                    for (int iy = 1; iy < _dungeonHeight - 1; iy++)
                    {
                        IntVector2 pos = new IntVector2(ix, iy);

                        if (getTile(pos) == Tile.Wall) continue;

                        // If it only has one exit, it's a dead end.
                        var exits = 0;
                        foreach (IntVector2 dir in cardinal)
                        {
                            if (getTile(pos + dir) != Tile.Wall) exits++;
                        }

                        if (exits != 1) continue;

                        done = false;
                        setTile(pos, Tile.Wall);
                        _regions[pos.x, pos.y] = -1;
                    }
                }
            }
        }

        private void _streamLineCorridors()
        {
            /*Added by Marrt taken from this user comment on the source page:
            Peeling • 7 months ago
            As regards the disagreeable windiness between rooms: looking at the output you could get rid of most of it thus:
            Trace each linear corridor section (terminated by branches or rooms)
            Once you have the start and end of a section, retrace your steps. If you find a point where you could dig through one block to make a shortcut to an earlier part of the section, do so, and fill in the unwanted part. Continue until you reach the start of the section.
            Repeat for all linear corridor sections.
        */

            //STEP 1: gather all Tiles.Floor, these are all corridor Tiles
            List<IntVector2> corridors = new List<IntVector2>();
            List<List<IntVector2>> traces = new List<List<IntVector2>>();

            for (int ix = 1; ix < _dungeonWidth - 1; ix++)
            {
                for (int iy = 1; iy < _dungeonHeight - 1; iy++)
                {
                    if (_dungeon[ix, iy] == Tile.Floor)
                    {
                        corridors.Add(new IntVector2(ix, iy));
                    }
                }
            }

            //STEP 2: gather corridor traces, these are all line segments that are between doors or branching points which themselves are fixed now		
            //extract Line Segments seperated by branching points or doorsteps		

            int failsave = 1000;
            while (corridors.Count > 0 && failsave > 0)
            {
                if (failsave == 1)
                {
                    print("Marrt didn't expect this to happen");
                }

                failsave--;

                // See which adjacent cells are open.
                List<IntVector2> segment = new List<IntVector2>();
                IntVector2 current = corridors[0]; //arbitrary start		
                buildLineSegment(current, ref corridors, ref segment, 0, true); //recursive search

                if (segment.Count > 4)
                {
                    //lineSegment has to have at least 5 parts to potentially contain a shortcut
                    traces.Add(segment);
                    //debug	//	int g = Random.Range(100,300);	foreach(IntVector2 pos in segment){	_regions[pos.x, pos.y] = g;	}
                }
            }

            //STEP 3: backtrace traces and check for shortcuts within short range (1 wall in between), then carve a shortcut and uncarve the trace up to that point		
            foreach (List<IntVector2> trace in traces)
            {
                List<IntVector2> finalTrace = new List<IntVector2>();
                int skipIndex = 0; //shortcut skips iterations			

                for (int i = 0; i < trace.Count; i++)
                {
                    if (i < skipIndex)
                    {
                        continue;
                    }

                    finalTrace.Add(trace[i]); //add current position to final path

                    foreach (IntVector2 dir in cardinal)
                    {
                        if (getTile(trace[i] + dir) == Tile.Wall)
                        {
                            //if we see a wall in test direction

                            IntVector2 shortcut = trace[i] + dir + dir;
                            if (trace.Contains(shortcut) && !finalTrace.Contains(shortcut))
                            {
                                //and behind that wall an already visited pos of this trace that has not been removed

                                //get index of shortcut so we know how and if to skip
                                skipIndex = trace.FindIndex(delegate(IntVector2 x)
                                {
                                    return x == shortcut;
                                }); //implicit predicate							
                                if (i > skipIndex)
                                {
                                    continue;
                                } //detected an already obsolete path, we cannot make a shortcut to it

                                finalTrace.Add(trace[i] + dir); //new shortcut connection is added to final sum
                                //print ("shortcut"+i+"->"+skipIndex);
                            }
                        }
                    }
                }

                //uncarve old trace
                foreach (IntVector2 pos in trace)
                {
                    setTile(pos, Tile.Wall);
                    _regions[pos.x, pos.y] = -1;
                }

                //recarve trace
                foreach (IntVector2 pos in finalTrace)
                {
                    _carve(pos);
                    _regions[pos.x, pos.y] = 100;
                }
            }
        }

        //recursive line builder
        private int buildLineSegment(IntVector2 current, ref List<IntVector2> source, ref List<IntVector2> target,
            int currentDepth, bool addAtEnd)
        {
            if (currentDepth > 1000)
            {
                return currentDepth + 1;
            } //failsave

            //check if we are a doorstep or branch, these must not be moved or else
            int exits = 0;
            foreach (IntVector2 dir in cardinal)
            {
                if (getTile(current + dir) != Tile.Wall)
                {
                    //if there is anything other than a wall we have an exit or else, doorsteps will have at least 2 non walls (door + path)
                    exits++;
                }
            }

            if (exits > 2)
            {
                source.Remove(current); //never look at this tile again
                return currentDepth;
            }
            else
            {
                if (addAtEnd)
                {
                    target.Insert(0, current); //at least part of a valid lineSegment
                }
                else
                {
                    target.Add(current);
                }
            }

            //find adjacent fields, there are only up to 2 directions possible on any lineSegment point, we can only ever find one valid after the first		
            foreach (IntVector2 dir in cardinal)
            {
                if (source.Contains(current + dir) && !target.Contains(current + dir))
                {
                    //depth first
                    currentDepth = buildLineSegment(current + dir, ref source, ref target, currentDepth, addAtEnd);
                    //only first call can run twice because it may start in the middle of a segment
                    addAtEnd = false; //we want an ordered list, so initial depthsearch will be added at start, the following at the end
                }
            }

            source.Remove(current);
            return currentDepth + 1;
        }


        ///<summary>multiply hauberk dungeon tiles to make corridors broader, added by marrt</summary>
        private void MultiplyDungeon(int factor)
        {
            int xSize = _dungeon.GetLength(0);
            int ySize = _dungeon.GetLength(1);

            Tile[,] newDungeon = new Tile[xSize * factor, ySize * factor];
            int[,] newRegions = new int [xSize * factor, ySize * factor];


            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    int nx = 0;
                    int ny = 0;
                    for (int i = 0; i < factor * factor; i++)
                    {
                        newDungeon[x * factor + nx, y * factor + ny] = _dungeon[x, y];
                        newRegions[x * factor + nx, y * factor + ny] = _regions[x, y];
                        if (nx == factor - 1)
                        {
                            ny++;
                        }

                        nx = (nx + 1) % factor;
                    }
                }
            }

            _dungeon = newDungeon;
            _regions = newRegions;
        }


        /// Gets whether or not an opening can be carved from the given starting
        /// [Cell] at [pos] to the adjacent Cell facing [direction]. Returns `true`
        /// if the starting Cell is in bounds and the destination Cell is filled
        /// (or out of bounds).</returns>
        private bool _canCarve(IntVector2 pos, IntVector2 direction)
        {
            // Must end in bounds.

            IntVector2 iv2 = pos + direction * 3;
            Vector2 v2 = new Vector2(iv2.x, iv2.y);
            Rect bounds = new Rect(0, 0, _dungeonWidth, _dungeonHeight);

            if (!bounds.Contains(v2)) return false;

            // Destination must not be open.
            return getTile(pos + direction * 2) == Tile.Wall;
        }

        private void _startRegion()
        {
            _currentRegion++;
        }

        private void _carve(IntVector2 pos, Tile? type = null)
        {
            setTile(pos, type ?? Tile.Floor); // if non is stated, default is floor

            //print (pos.x +","+ pos.y);
            _regions[pos.x, pos.y] = _currentRegion;
        }

        #endregion


        #region Perlin Noise

        [Header("	Perlin Noise Dungeon")] public int offsetX = 23;
        public int offsetY = 23;
        public float scale = 0.1F;
        public float threshold = 0.5F;

        /// <summary>Generate a perlin noise dungeon, warning: no connectivity guaranteed </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="offsetX">x offset on the perlin sample plane</param>
        /// <param name="offsetY">y offset on the perlin sample plane</param>
        /// <param name="scale">bigger values make finer structures</param>
        /// <param name="threshold">0F-1F, threshold on when to draw a wall instead of a floor</param>
        /// 	
        public Dungeon GeneratePerlinDungeon(int? width = null,
            int? height = null,
            int? offsetX = null,
            int? offsetY = null,
            float? scale = null,
            float? threshold = null)
        {
            PerformanceMeter.StartTimeLog("tilegrid generation:\t");

            _dungeonWidth = width ?? _dungeonWidth;
            _dungeonHeight = height ?? _dungeonHeight;

            this.offsetX = offsetX ?? this.offsetX;
            this.offsetY = offsetY ?? this.offsetY;
            this.scale = scale ?? this.scale;
            this.threshold = threshold ?? this.threshold;

            //init
            _dungeon = new Tile[_dungeonWidth, _dungeonHeight];
            _regions = new int[_dungeonWidth, _dungeonHeight];

            for (int x = 0; x < _dungeonWidth; x++)
            {
                for (int y = 0; y < _dungeonHeight; y++)
                {
                    float noise = Mathf.PerlinNoise((x + this.offsetX) * this.scale, (y + this.offsetY) * this.scale);
                    if (noise < this.threshold)
                    {
                        _dungeon[x, y] = Tile.Floor;
                    }
                    else
                    {
                        _dungeon[x, y] = Tile.Wall;
                    }
                }
            }

            PerformanceMeter.EndTimeLog();
            PerformanceMeter.AddLog("");

            Dungeon dungeon = new Dungeon(_dungeon, _regions);

            dungeon.entrance = FindFloorInDungeon(dungeon);
            dungeon.entranceSet = true;

            UpdateMap(ref _dungeon);

            return dungeon;
        }

        #endregion


        #region Cave

        //https://github.com/AndyStobirski/RogueLike/blob/master/csCaveGenerator.cs

        /// <summary>Generate a cave dungeon, http://www.evilscience.co.uk/a-c-algorithm-to-build-roguelike-cave-systems-part-1/ </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="Neighbours">The number of closed neighbours a cell must have in order to invert it's state</param>
        /// <param name="CloseCellProb">The probability of closing a visited cell, 55 tends to produce 1 cave, 40 few and small caves</param>
        /// <param name="Iterations">The number of times to visit cells</param>
        /// <param name="LowerLimit">Remove rooms smaller than this value</param>
        /// <param name="UpperLimit">Remove rooms larger than this value</param>
        /// <param name="EmptyNeighbours">Removes single cells from cave edges: a cell with this number of empty neighbours is removed</param>
        /// <param name="EmptyCellNeighbours">Fills in holes within caves: an open cell with this number closed neighbours is filled</param>
        /// <param name="seed">use random seed for this generation, ignores _randomSeedOn</param>
        public Dungeon GenerateCaveDungeon(int? width = null,
            int? height = null,
            int? Neighbours = null,
            int? CloseCellProb = null,
            int? Iterations = null,
            int? LowerLimit = null,
            int? UpperLimit = null,
            int? EmptyNeighbours = null,
            int? EmptyCellNeighbours = null,
            int? seed = null)
        {
            PerformanceMeter.StartTimeLog("tilegrid generation:\t");

            _dungeonWidth = width ?? _dungeonWidth;
            _dungeonHeight = height ?? _dungeonHeight;

            this.Neighbours = Neighbours ?? this.Neighbours;
            this.CloseCellProb = CloseCellProb ?? this.CloseCellProb;
            this.Iterations = Iterations ?? this.Iterations;
            this.LowerLimit = LowerLimit ?? this.LowerLimit;
            this.UpperLimit = UpperLimit ?? this.UpperLimit;
            this.EmptyNeighbours = EmptyNeighbours ?? this.EmptyNeighbours;
            this.EmptyNeighbours = EmptyNeighbours ?? this.EmptyNeighbours;
            this.EmptyCellNeighbours = EmptyCellNeighbours ?? this.EmptyCellNeighbours;

            this._seed = seed ?? this._seed;

            if (_randomSeedOn || seed != null)
            {
                //use random seed if overlaod was used, or if it is enabled in general
                Random.seed = this._seed;
            }

            this.MapSize = new IntVector2(_dungeonWidth, _dungeonHeight);

            CaveBuild();
            ConnectCaves();

            _dungeon = new Tile[_dungeonWidth, _dungeonHeight];
            _regions = new int[_dungeonWidth, _dungeonHeight];

            for (int x = 0; x < _dungeonWidth; x++)
            {
                for (int y = 0; y < _dungeonHeight; y++)
                {
                    _dungeon[x, y] = Map[x, y] == 1 ? Tile.Floor : Tile.Wall;
                    _regions[x, y] = Map[x, y] == 1 ? 0 : -1;
                }
            }

            PerformanceMeter.EndTimeLog();
            PerformanceMeter.AddLog("");

            Dungeon dungeon = new Dungeon(_dungeon, _regions);

            dungeon.entrance = FindFloorInDungeon(dungeon);
            dungeon.entranceSet = true;

            UpdateMap(ref _dungeon);

            return dungeon;
        }


        /// <summary>
        /// csCaveGenerator - generate a cave system and connect the caves together.
        /// 
        /// For more info on it's use see http://www.evilscience.co.uk/?p=624
        /// </summary>
        /// 
//Marrt: i removed the class distinction to make those parameters visibe in the Editor
        //class csCaveGenerator{

        #region properties

        [Header("Cave Generation")] [Header("	Cave Dungeon")]
        public int Neighbours = 4; //The number of closed neighbours a cell must have in order to invert it's state

        public int
            CloseCellProb =
                45; //The probability of closing a visited cell, 55 tends to produce 1 cave, 40 few and small caves

        public int Iterations = 50000; //The number of times to visit cells
        public IntVector2 MapSize; //The size of the map		

        [Header("Cave Cleaning")] public int LowerLimit = 16; //Remove rooms smaller than this value
        public int UpperLimit = 500; //Remove rooms larger than this value

        public int
            EmptyNeighbours =
                3; //Removes single cells from cave edges: a cell with this number of empty neighbours is removed

        public int
            EmptyCellNeighbours =
                4; //Fills in holes within caves: an open cell with this number closed neighbours is filled

        [Header("Corridor")] public int Corridor_Min = 2; //Minimum corridor length
        public int Corridor_Max = 5; //Maximum corridor length
        public int Corridor_MaxTurns = 10; //Maximum turns
        public int CorridorSpace = 2; //The distance a corridor has to be away from a closed cell for it to be built

        public int
            BreakOut = 100000; //When this value is exceeded, stop attempting to connect caves. Prevents the algorithm from getting stuck.

        private int CaveNumber
        {
            get { return Caves == null ? 0 : Caves.Count; }
        } //Number of caves generated

        #endregion

        #region map structures

        private List<List<IntVector2>> Caves; // Caves within the map are stored here
        private List<IntVector2> Corridors; // Corridors within the map stored here
        private int[,] Map; // Contains the map

        #endregion

        #region lookups

        /// <summary>Generic list of points which contain 4 directions </summary>
        List<IntVector2> Directions = new List<IntVector2>()
        {
            new IntVector2(0, -1) //north
            ,
            new IntVector2(0, 1) //south
            ,
            new IntVector2(1, 0) //east
            ,
            new IntVector2(-1, 0) //west
        };

        List<IntVector2> Directions8 = new List<IntVector2>()
        {
            new IntVector2(0, -1) //north
            ,
            new IntVector2(0, 1) //south
            ,
            new IntVector2(1, 0) //east
            ,
            new IntVector2(-1, 0) //west
            ,
            new IntVector2(1, -1) //northeast
            ,
            new IntVector2(-1, -1) //northwest
            ,
            new IntVector2(-1, 1) //southwest
            ,
            new IntVector2(1, 1) //southeast
            ,
            new IntVector2(0, 0) //centre
        };

        #endregion

        #region misc

        /// <summary>
        /// Constructor
        /// </summary>
        /*public csCaveGenerator(){
        
        rnd = Random(12345);
        Neighbours = 4;
        Iterations = 50000;
        CloseCellProb = 45;

        LowerLimit = 16;
        UpperLimit = 500;

        MapSize = new IntVector2(40, 20);

        EmptyNeighbours = 3;
        EmptyCellNeighbours = 4;

        CorridorSpace = 2;
        Corridor_MaxTurns = 10;
        Corridor_Min = 2;
        Corridor_Max = 5;

        BreakOut = 100000;
    }*/
        public int CaveBuild()
        {
            BuildCaves();
            GetCaves();
            return Caves.Count();
        }

        #endregion


        #region cave related

        #region make caves

        /// <summary>
        /// Calling this method will build caves, smooth them off and fill in any holes
        /// </summary>
        private void BuildCaves()
        {
            Map = new int[MapSize.x, MapSize.y];


            //go through each map cell and randomly determine whether to close it
            //the +5 offsets are to leave an empty border round the edge of the map
            for (int x = 0; x < MapSize.x; x++)
            for (int y = 0; y < MapSize.y; y++)
                if (Random.Range(0, 100) < CloseCellProb)
                    Map[x, y] = 1;

            IntVector2 cell;

            //Pick cells at random
            for (int x = 0; x <= Iterations; x++)
            {
                cell = new IntVector2(Random.Range(0, MapSize.x), Random.Range(0, MapSize.y));

                //if the randomly selected cell has more closed neighbours than the property Neighbours
                //set it closed, else open it
                if (Neighbours_Get1(cell).Where(n => IntVector2_Get(n) == 1).Count() > Neighbours)
                    IntVector2_Set(cell, 1);
                else
                    IntVector2_Set(cell, 0);
            }


            //
            //  Smooth of the rough cave edges and any single blocks by making several 
            //  passes on the map and removing any cells with 3 or more empty neighbours
            //
            for (int ctr = 0; ctr < 5; ctr++)
            {
                //examine each cell individually
                for (int x = 0; x < MapSize.x; x++)
                for (int y = 0; y < MapSize.y; y++)
                {
                    cell = new IntVector2(x, y);

                    if (
                        IntVector2_Get(cell) > 0
                        && Neighbours_Get(cell).Where(n => IntVector2_Get(n) == 0).Count() >= EmptyNeighbours
                    )
                        IntVector2_Set(cell, 0);
                }
            }

            //
            //  fill in any empty cells that have 4 full neighbours
            //  to get rid of any holes in an cave
            //
            for (int x = 0; x < MapSize.x; x++)
            for (int y = 0; y < MapSize.y; y++)
            {
                cell = new IntVector2(x, y);

                if (
                    IntVector2_Get(cell) == 0
                    && Neighbours_Get(cell).Where(n => IntVector2_Get(n) == 1).Count() >= EmptyCellNeighbours
                )
                    IntVector2_Set(cell, 1);
            }
        }

        #endregion

        #region locate caves

        /// <summary>
        /// Locate the edge of the specified cave
        /// </summary>
        /// <param name="pCaveNumber">Cave to examine</param>
        /// <param name="pCavePoint">Point on the edge of the cave</param>
        /// <param name="pDirection">Direction to start formting the tunnel</param>
        /// <returns>Boolean indicating if an edge was found</returns>
        private void Cave_GetEdge(List<IntVector2> pCave, ref IntVector2 pCavePoint, ref IntVector2 pDirection)
        {
            do
            {
                //random point in cave
                pCavePoint = pCave.ToList()[Random.Range(0, pCave.Count())];

                pDirection = Direction_Get(pDirection);

                do
                {
                    pCavePoint += pDirection;

                    if (!IntVector2_Check(pCavePoint))
                        break;
                    else if (IntVector2_Get(pCavePoint) == 0)
                        return;
                } while (true);
            } while (true);
        }

        /// <summary>Locate all the caves within the map and place each one into the generic list Caves</summary>
        private void GetCaves()
        {
            Caves = new List<List<IntVector2>>();

            List<IntVector2> Cave;
            IntVector2 cell;

            //examine each cell in the map...
            for (int x = 0; x < MapSize.x; x++)
            for (int y = 0; y < MapSize.y; y++)
            {
                cell = new IntVector2(x, y);
                //if the cell is closed, and that cell doesn't occur in the list of caves..
                if (IntVector2_Get(cell) > 0 && Caves.Count(s => s.Contains(cell)) == 0)
                {
                    Cave = new List<IntVector2>();

                    //launch the recursive
                    LocateCave(cell, Cave);

                    //check that cave falls with the specified property range size...
                    if (Cave.Count() <= LowerLimit | Cave.Count() > UpperLimit)
                    {
                        //it does, so bin it
                        foreach (IntVector2 p in Cave)
                            IntVector2_Set(p, 0);
                    }
                    else
                        Caves.Add(Cave);
                }
            }
        }

        /// <summary>
        /// Recursive method to locate the cells comprising a cave, 
        /// based on flood fill algorithm
        /// </summary>
        /// <param name="cell">Cell being examined</param>
        /// <param name="current">List containing all the cells in the cave</param>
        private void LocateCave(IntVector2 pCell, List<IntVector2> pCave)
        {
            foreach (IntVector2 p in Neighbours_Get(pCell).Where(n => IntVector2_Get(n) > 0))
            {
                if (!pCave.Contains(p))
                {
                    pCave.Add(p);
                    LocateCave(p, pCave);
                }
            }
        }

        #endregion

        #region connect caves

        /// <summary>
        /// Attempt to connect the caves together
        /// </summary>
        public bool ConnectCaves()
        {
            if (Caves.Count() == 0)
                return false;


            List<IntVector2> currentcave;
            List<List<IntVector2>> ConnectedCaves = new List<List<IntVector2>>();
            IntVector2 cor_point = new IntVector2(0, 0);
            IntVector2 cor_direction = new IntVector2(0, 0);
            List<IntVector2> potentialcorridor = new List<IntVector2>();
            int breakoutctr = 0;

            Corridors = new List<IntVector2>(); //corridors built stored here

            //get started by randomly selecting a cave..
            currentcave = Caves[Random.Range(0, Caves.Count())];
            ConnectedCaves.Add(currentcave);
            Caves.Remove(currentcave);


            //starting builder
            do
            {
                //no corridors are present, sp build off a cave
                if (Corridors.Count() == 0)
                {
                    currentcave = ConnectedCaves[Random.Range(0, ConnectedCaves.Count())];
                    Cave_GetEdge(currentcave, ref cor_point, ref cor_direction);
                }
                else
                    //corridors are presnt, so randomly chose whether a get a start
                    //point from a corridor or cave
                if (Random.Range(0, 100) > 49)
                {
                    currentcave = ConnectedCaves[Random.Range(0, ConnectedCaves.Count())];
                    Cave_GetEdge(currentcave, ref cor_point, ref cor_direction);
                }
                else
                {
                    currentcave = null;
                    Corridor_GetEdge(ref cor_point, ref cor_direction);
                }


                //using the points we've determined above attempt to build a corridor off it
                potentialcorridor = Corridor_Attempt(cor_point
                    , cor_direction
                    , true);


                //if not null, a solid object has been hit
                if (potentialcorridor != null)
                {
                    //examine all the caves
                    for (int ctr = 0; ctr < Caves.Count(); ctr++)
                    {
                        //check if the last point in the corridor list is in a cave
                        if (Caves[ctr].Contains(potentialcorridor.Last()))
                        {
                            if (
                                currentcave == null //we've built of a corridor
                                | currentcave != Caves[ctr] //or built of a room
                            )
                            {
                                //the last corridor point intrudes on the room, so remove it
                                potentialcorridor.Remove(potentialcorridor.Last());
                                //add the corridor to the corridor collection
                                Corridors.AddRange(potentialcorridor);
                                //write it to the map
                                foreach (IntVector2 p in potentialcorridor)
                                    IntVector2_Set(p, 1);


                                //the room reached is added to the connected list...
                                ConnectedCaves.Add(Caves[ctr]);
                                //...and removed from the Caves list
                                Caves.RemoveAt(ctr);

                                break;
                            }
                        }
                    }
                }

                //breakout
                if (breakoutctr++ > BreakOut)
                    return false;
            } while (Caves.Count() > 0);

            Caves.AddRange(ConnectedCaves);
            ConnectedCaves.Clear();
            return true;
        }

        #endregion

        #endregion

        #region corridor related

        /// <summary>
        /// Randomly get a point on an existing corridor
        /// </summary>
        /// <param name="Location">Out: location of point</param>
        /// <returns>Bool indicating success</returns>
        private void Corridor_GetEdge(ref IntVector2 pLocation, ref IntVector2 pDirection)
        {
            List<IntVector2> validdirections = new List<IntVector2>();

            do
            {
                //the modifiers below prevent the first of last point being chosen
                pLocation = Corridors[Random.Range(0, Corridors.Count - 1)];

                //attempt to locate all the empy map points around the location
                //using the directions to offset the randomly chosen point
                foreach (IntVector2 p in Directions)
                    if (IntVector2_Check(new IntVector2(pLocation.x + p.x, pLocation.y + p.y)))
                        if (IntVector2_Get(new IntVector2(pLocation.x + p.x, pLocation.y + p.y)) == 0)
                            validdirections.Add(p);
            } while (validdirections.Count == 0);

            pDirection = validdirections[Random.Range(0, validdirections.Count)];
            pLocation += pDirection;
        }

        /// <summary>
        /// Attempt to build a corridor
        /// </summary>
        /// <param name="pStart"></param>
        /// <param name="pDirection"></param>
        /// <param name="pPreventBackTracking"></param>
        /// <returns></returns>
        private List<IntVector2> Corridor_Attempt(IntVector2 pStart, IntVector2 pDirection, bool pPreventBackTracking)
        {
            List<IntVector2> lPotentialCorridor = new List<IntVector2>();
            lPotentialCorridor.Add(pStart);

            int corridorlength;
            IntVector2 startdirection = new IntVector2(pDirection.x, pDirection.y);

            int pTurns = Corridor_MaxTurns;

            while (pTurns >= 0)
            {
                pTurns--;

                corridorlength = Random.Range(Corridor_Min, Corridor_Max);
                //build corridor
                while (corridorlength > 0)
                {
                    corridorlength--;

                    //make a point and offset it
                    pStart += pDirection;

                    if (IntVector2_Check(pStart) && IntVector2_Get(pStart) == 1)
                    {
                        lPotentialCorridor.Add(pStart);
                        return lPotentialCorridor;
                    }

                    if (!IntVector2_Check(pStart))
                        return null;
                    else if (!Corridor_IntVector2Test(pStart, pDirection))
                        return null;

                    lPotentialCorridor.Add(pStart);
                }

                if (pTurns > 1)
                    if (!pPreventBackTracking)
                        pDirection = Direction_Get(pDirection);
                    else
                        pDirection = Direction_Get(pDirection, startdirection);
            }

            return null;
        }

        private bool Corridor_IntVector2Test(IntVector2 pPoint, IntVector2 pDirection)
        {
            //using the property corridor space, check that number of cells on
            //either side of the point are empty
            foreach (int r in Enumerable.Range(-CorridorSpace, 2 * CorridorSpace + 1).ToList())
            {
                if (pDirection.x == 0) //north or south
                {
                    if (IntVector2_Check(new IntVector2(pPoint.x + r, pPoint.y)))
                        if (IntVector2_Get(new IntVector2(pPoint.x + r, pPoint.y)) != 0)
                            return false;
                }
                else if (pDirection.y == 0) //east west
                {
                    if (IntVector2_Check(new IntVector2(pPoint.x, pPoint.y + r)))
                        if (IntVector2_Get(new IntVector2(pPoint.x, pPoint.y + r)) != 0)
                            return false;
                }
            }

            return true;
        }

        #endregion

        #region direction related

        /// <summary>
        /// Return a list of the valid neighbouring cells of the provided point
        /// using only north, south, east and west
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private List<IntVector2> Neighbours_Get(IntVector2 p)
        {
            return Directions.Select(d => new IntVector2(p.x + d.x, p.y + d.y))
                .Where(d => IntVector2_Check(d)).ToList();
        }

        /// <summary>
        /// Return a list of the valid neighbouring cells of the provided point
        /// using north, south, east, ne,nw,se,sw
        private List<IntVector2> Neighbours_Get1(IntVector2 p)
        {
            return Directions8.Select(d => new IntVector2(p.x + d.x, p.y + d.y))
                .Where(d => IntVector2_Check(d)).ToList();
        }

        /// <summary>
        /// Get a random direction, provided it isn't equal to the opposite one provided
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private IntVector2 Direction_Get(IntVector2 p)
        {
            IntVector2 newdir;
            do
            {
                newdir = Directions[Random.Range(0, Directions.Count())];
            } while (newdir.x != -p.x & newdir.y != -p.y);

            return newdir;
        }

        /// <summary>
        /// Get a random direction, excluding the provided directions and the opposite of 
        /// the provided direction to prevent a corridor going back on it's self.
        /// 
        /// The parameter pDirExclude is the first direction chosen for a corridor, and
        /// to prevent it from being used will prevent a corridor from going back on 
        /// it'self
        /// </summary>
        /// <param name="dir">Current direction</param>
        /// <param name="pDirectionList">Direction to exclude</param>
        /// <param name="pDirExclude">Direction to exclude</param>
        /// <returns></returns>
        private IntVector2 Direction_Get(IntVector2 pDir, IntVector2 pDirExclude)
        {
            IntVector2 NewDir;
            do
            {
                NewDir = Directions[Random.Range(0, Directions.Count())];
            } while (
                Direction_Reverse(NewDir) == pDir
                | Direction_Reverse(NewDir) == pDirExclude
            );


            return NewDir;
        }

        private IntVector2 Direction_Reverse(IntVector2 pDir)
        {
            return new IntVector2(-pDir.x, -pDir.y);
        }

        #endregion

        #region cell related

        /// <summary>
        /// Check if the provided point is valid
        /// </summary>
        /// <param name="p">Point to check</param>
        /// <returns></returns>
        private bool IntVector2_Check(IntVector2 p)
        {
            return p.x >= 0 & p.x < MapSize.x & p.y >= 0 & p.y < MapSize.y;
        }

        /// <summary>
        /// Set the map cell to the specified value
        /// </summary>
        /// <param name="p"></param>
        /// <param name="val"></param>
        private void IntVector2_Set(IntVector2 p, int val)
        {
            Map[p.x, p.y] = val;
        }

        /// <summary>
        /// Get the value of the provided point
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        private int IntVector2_Get(IntVector2 p)
        {
            return Map[p.x, p.y];
        }

        #endregion

        //}	

        #endregion


        #region MAP

        [Header("	Map")] public Image mapImage;
        [HideInInspector] public Texture2D mapTexture;
        public Canvas canvas;

        //create map
        private void CreateMap()
        {
            if (mapImage)
            {
                return;
            } //if already created, leave, update is enough

            //requires Canvas			
            //create new SpriteGameObject in UI
            var mapGo = new GameObject {name = "MapObject", layer = LayerMask.NameToLayer("UI")};
            //give it a name
            //culling layer, if needed

            var sRect = mapGo.AddComponent<RectTransform>(); //add a rect Transform to replace the normal Transform

            //Assign in Editor now//Canvas canvas = (Canvas)FindObjectOfType(typeof(Canvas));		
            //if (!canvas){print("No Canvas found, Map will not be drawn"); return;}

            sRect.SetParent(canvas.gameObject.transform,
                false); //make the image Object the new parent, so this will move with it		

            mapImage = mapGo.AddComponent<Image>(); //Add a sprite Renderer to the new Object	
            mapImage.enabled = false;

            mapTexture = new Texture2D(1, 1, TextureFormat.RGBA32, false) {filterMode = FilterMode.Point};

            //Add Button to Map to toggle region view
            if (addMapToggle)
            {
                var b = mapImage.gameObject.AddComponent<Button>();
                b.onClick.AddListener(ToggleMap);
            }
        }

        //update map by region grid
        private void UpdateMap(ref int[,] grid)
        {
            if (mapTexture == null)
            {
                return;
            }

            int xSize = grid.GetLength(0);
            int ySize = grid.GetLength(1);

            mapTexture.Resize(xSize, ySize, TextureFormat.RGBA32, false);
            mapTexture.Apply();

            //generate random Colors
            var colors = new Dictionary<int, Color>();
            for (var x = 0; x < xSize; x++)
            {
                for (var y = 0; y < ySize; y++)
                {
                    if (!colors.ContainsKey(grid[x, y]))
                    {
                        colors.Add(grid[x, y],
                            new Color(Random.Range(0F, 1F), Random.Range(0F, 1F), Random.Range(0F, 1F)));
                    }
                }
            }

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    mapTexture.SetPixel(x, y, grid[x, y] == -1 ? Color.black : colors[grid[x, y]]);

                    //special Debug case, -100
                    if (grid[x, y] == -100) mapTexture.SetPixel(x, y, Color.white);
                }
            }

            mapTexture.Apply();
            RefreshMap(new Vector2(xSize, ySize));
        }

        //update map by Tilegrid
        public void UpdateMap(ref Tile[,] grid)
        {
            var xSize = grid.GetLength(0);
            var ySize = grid.GetLength(1);

            mapTexture.Resize(xSize, ySize, TextureFormat.RGBA32, false);
            mapTexture.Apply();

            for (var x = 0; x < xSize; x++)
            {
                for (var y = 0; y < ySize; y++)
                {
                    switch (grid[x, y])
                    {
                        case Tile.None:
                            break;
                        case Tile.Hole:
                            break;
                        case Tile.OpenDoor:
                            break;
                        case Tile.ClosedDoor:
                            break;
                        case Tile.Spawner:
                            break;
                        default:
                            mapTexture.SetPixel(x, y, Color.clear);
                            break;
                        case Tile.Floor:
                            mapTexture.SetPixel(x, y, Color.grey);
                            break;
                        case Tile.RoomFloor:
                            mapTexture.SetPixel(x, y, Color.white);
                            break;
                        case Tile.Wall:
                            mapTexture.SetPixel(x, y, Color.black);
                            break;
                        case Tile.Door:
                            mapTexture.SetPixel(x, y, Color.magenta);
                            break;

                        //DEBUG for room Template Dungeon
                        case Tile.DoorNorth:
                            mapTexture.SetPixel(x, y, Color.yellow);
                            break;
                        case Tile.DoorEast:
                            mapTexture.SetPixel(x, y, Color.blue);
                            break;
                        case Tile.DoorSouth:
                            mapTexture.SetPixel(x, y, Color.green);
                            break;
                        case Tile.DoorWest:
                            mapTexture.SetPixel(x, y, Color.cyan);
                            break;
                    }
                }
            }

            mapTexture.Apply();
            RefreshMap(new Vector2(xSize, ySize));
        }

        // http://wiki.unity3d.com/index.php?title=TextureDrawLine
        private void PrintLineOnMap(int x0, int y0, int x1, int y1, Color color, float blendFactor)
        {
            int dy = (int) (y1 - y0);
            int dx = (int) (x1 - x0);
            int stepx, stepy;

            if (dy < 0)
            {
                dy = -dy;
                stepy = -1;
            }
            else
            {
                stepy = 1;
            }

            if (dx < 0)
            {
                dx = -dx;
                stepx = -1;
            }
            else
            {
                stepx = 1;
            }

            dy <<= 1;
            dx <<= 1;

            float fraction = 0;

            BlendMapPixel(x0, y0, color, blendFactor);
            if (dx > dy)
            {
                fraction = dy - (dx >> 1);
                while (Mathf.Abs(x0 - x1) > 1)
                {
                    if (fraction >= 0)
                    {
                        y0 += stepy;
                        fraction -= dx;
                    }

                    x0 += stepx;
                    fraction += dy;
                    BlendMapPixel(x0, y0, color, blendFactor);
                }
            }
            else
            {
                fraction = dx - (dy >> 1);
                while (Mathf.Abs(y0 - y1) > 1)
                {
                    if (fraction >= 0)
                    {
                        x0 += stepx;
                        fraction -= dy;
                    }

                    y0 += stepy;
                    fraction += dx;
                    BlendMapPixel(x0, y0, color, blendFactor);
                }
            }

            mapTexture.Apply();
        }

        public void ColorRoomOnMap(Room room, Color color, float blendFactor)
        {
            int xSize = room.tiles.GetLength(0);
            int ySize = room.tiles.GetLength(1);
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (room.tiles[x, y] == Tile.Floor)
                    {
                        BlendMapPixel(x + room.x, y + room.y, color, blendFactor);
                    }
                }
            }
        }

        public void SetMapPixel(int x, int y, Color color)
        {
            mapTexture.SetPixel(x, y, color);
        }

        public void BlendMapPixel(int x, int y, Color color, float factor)
        {
            Color color1 = mapTexture.GetPixel(x, y);
            mapTexture.SetPixel(x, y, Color.Lerp(color1, color, factor));
        }

        public Color GetMapPixel(int x, int y)
        {
            return mapTexture.GetPixel(x, y);
        }

        public bool showMap = true;

        private float mapPxPerTile = 1F;

        private void RefreshMap(Vector2 sizePx)
        {
            //keep order of this changes:		
            mapImage.rectTransform.anchorMin = new Vector2(1F, 0F);
            mapImage.rectTransform.anchorMax = new Vector2(1F, 0F);
            mapImage.rectTransform.pivot = new Vector2(1F, 0F);
            mapImage.rectTransform.offsetMin = Vector2.zero;
            mapImage.rectTransform.offsetMax = sizePx * mapPxPerTile; //1 tile = 2x2 px
            mapImage.rectTransform.anchoredPosition = new Vector2(-3, +3); //small dist from corner	

            Sprite sprite = Sprite.Create(mapTexture, new Rect(0, 0, mapTexture.width, mapTexture.height),
                new Vector2(0.5F, 0.5F));
            mapImage.sprite = sprite;
            mapImage.enabled = showMap;
        }

        private bool regView = false;

        public void ToggleMap()
        {
            regView = !regView;
            if (regView)
            {
                UpdateMap(ref _regions);
            }
            else
            {
                UpdateMap(ref _dungeon);
            }
        }

        //gridX/Y is the gameUnit dimension of your Tile, made for top-down default = XZ-Plane
        public void ShowTransformOnMap(Transform actor, float gridX, float gridY, float? mapOrigX = null,
            float? mapOrigY = null)
        {
            float offX = mapOrigX ?? default(float);
            float offY = mapOrigY ?? default(float);

            StartCoroutine(ShowOnMap(actor, gridX, gridY, offX, offY));
        }

        public int actorPosX = 0;
        public int actorPosY = 0;
        private bool mapPointerActive = false;

        private IEnumerator ShowOnMap(Transform actor, float gridX, float gridY, float mapOffX, float mapOffY)
        {
            if (mapPointerActive)
            {
                mapPointerActive = false;
                yield return null;
            } //finish current coroutine

            //print(mapOffX+"|"+mapOffY);

            mapPointerActive = true;

            bool firstEnter = true; //don't color pixel on enter, we would color 0/0
            Color prevColor = Color.yellow;

            while (mapPointerActive)
            {
                int actorPixelX = (int) (Mathf.Round((actor.position.x - mapOffX) / gridX));
                int actorPixelY = (int) (Mathf.Round((actor.position.z - mapOffY) / gridY));

                //print(actorPixelX+"|"+actorPixelY);

                //only update if needed
                if (actorPosX != actorPixelX || actorPosY != actorPixelY)
                {
                    if (firstEnter)
                    {
                        firstEnter = false;
                    }
                    else
                    {
                        SetMapPixel(actorPosX, actorPosY, prevColor); //resetPrevious Pixel
                        BlendMapPixel(actorPosX, actorPosY, Color.green, 0.3F); //add trail tp prev
                    }

                    actorPosX = actorPixelX;
                    actorPosY = actorPixelY;

                    prevColor = GetMapPixel(actorPosX, actorPosY);
                    SetMapPixel(actorPosX, actorPosY, Color.red); //Highlight Map

                    mapTexture.Apply();
                }

                yield return null;
            }
        }

        #endregion
    }
}