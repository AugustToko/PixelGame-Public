using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PixelGameAssets.Scripts.Unused
{ // needed for List<>

    #region CLASSES

    public class Dungeon
    {
        public Tile[,] tiles;
        public int[,] regions;

        public IntVector2 entrance;
        public IntVector2 exit;

        public bool entranceSet = false;
        public bool exitSet = false;

        //additional parameter set to specify more characteristics of the dungeon
        public DungeonParameters parameters { get; private set; }

        protected void SetDungeonParameters(DungeonParameters parameters)
        {
            this.parameters = parameters;
        } //used by children to set

        public List<Portal> portals = null;

        public Dungeon()
        {
        }

        public Dungeon(Tile[,] tiles, int[,] regions)
        {
            this.tiles = tiles;
            this.regions = regions;
        }

        public Dungeon(Tile[,] tiles, int[,] regions, IntVector2 entrance, IntVector2 exit)
        {
            this.tiles = tiles;
            this.regions = regions;

            this.entrance = entrance;
            this.exit = exit;
        }

        //http://stackoverflow.com/questions/42519/how-do-you-rotate-a-two-dimensional-array
        static int[,] RotateMatrix(int[,] matrix, int n)
        {
            int[,] ret = new int[n, n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i, j] = matrix[n - j - 1, i];
                }
            }

            return ret;
        }
    }

    public enum Tile
    {
        None,
        Hole,
        Floor,
        RoomFloor,
        Wall,
        Door,
        OpenDoor,
        ClosedDoor,
        DoorNorth,
        DoorEast,
        DoorSouth,
        DoorWest,
        Spawner
    };


    public class DungeonParameters
    {
        public int width; // width of the Dungeon
        public int height; // height of the Dungeon

        public int minimumRoomCount; // minimum amount of rooms that are needed

        public List<PortalTemplate> portalTemplates; //list of gateways, all gateways must be instantiated

        public DungeonParameters(
            int width,
            int height,
            int minimumRoomCount,
            List<PortalTemplate> portalTemplates
        )
        {
            this.width = width;
            this.height = height;
            this.minimumRoomCount = minimumRoomCount;
            this.portalTemplates = portalTemplates;
        }
    }

    public class PortalTemplate
    {
        public string stringID; //id, if two dungeons have a portal with the same id they are connected through it
        public PathRole pathrole;
        public PortalDirection direction { get; private set; } // direction of this gateway


        public PortalTemplate(
            string stringID,
            PathRole pathrole,
            PortalDirection direction
        )
        {
            this.stringID = stringID;
            this.pathrole = pathrole;
            this.direction = direction;
        }
    }


    public class Portal
    {
        public PortalTemplate template { get; private set; }

        public IntVector2 position = new IntVector2(-1, -1); //position in Dungeon
        public GameObject gatewayObject;

        public PortalDirection direction;

        public int x
        {
            get { return position.x; }
            private set { }
        }

        public int y
        {
            get { return position.y; }
            private set { }
        }

        public string GetPortalInfo()
        {
            string info = "[";
            info += "id:" + this.template.stringID;
            info += ",\t";
            info += "role:" + System.Enum.GetName(typeof(PathRole), this.template.pathrole);
            info += ",\t";
            info += "dir:" + System.Enum.GetName(typeof(PortalDirection), this.direction);
            info += ",\t";
            info += "(dirT):" + System.Enum.GetName(typeof(PortalDirection), this.template.direction);
            info += ",\t";
            info += "pos:" + position;
            info += "]";
            return info;
        }


        /// <summary>
        /// Used when adding a gateway to a dungeon based on an instance of PortalTemplate
        /// </summary>
        public Portal(PortalTemplate template)
        {
            this.template = template;
            this.direction = template.direction; //can get modified
        }
    }

    public enum PortalDirection
    {
        None,
        Any,
        North,
        East,
        South,
        West
    }; //Bridge direction, like Doordirection: East means entering from west

    public static class DungeonExtensions
    {
        public static PortalDirection GetRotated(this PortalDirection portalDirection)
        {
            //rotate matehmatically positive
            switch (portalDirection)
            {
                default:
                case PortalDirection.None: return PortalDirection.None;
                case PortalDirection.Any: return PortalDirection.Any;

                case PortalDirection.North: return PortalDirection.West;
                case PortalDirection.East: return PortalDirection.North;
                case PortalDirection.South: return PortalDirection.East;
                case PortalDirection.West: return PortalDirection.South;
            }
        }

        public static DoorDirection GetDoorDirection(this PortalDirection portalDirection)
        {
            switch (portalDirection)
            {
                default:
                case PortalDirection.None: return DoorDirection.North;
                case PortalDirection.Any: return DoorDirection.North;

                case PortalDirection.North: return DoorDirection.North;
                case PortalDirection.East: return DoorDirection.East;
                case PortalDirection.South: return DoorDirection.South;
                case PortalDirection.West: return DoorDirection.West;
            }
        }

        public static PortalDirection GetOpposite(this PortalDirection portalDirection)
        {
            switch (portalDirection)
            {
                default:
                case PortalDirection.None: return PortalDirection.Any;
                case PortalDirection.Any: return PortalDirection.Any;

                case PortalDirection.North: return PortalDirection.South;
                case PortalDirection.East: return PortalDirection.West;
                case PortalDirection.South: return PortalDirection.North;
                case PortalDirection.West: return PortalDirection.East;
            }
        }

        public static DoorDirection GetOpposite(this DoorDirection portalDirection)
        {
            switch (portalDirection)
            {
                default:
                case DoorDirection.North: return DoorDirection.South;
                case DoorDirection.East: return DoorDirection.West;
                case DoorDirection.South: return DoorDirection.North;
                case DoorDirection.West: return DoorDirection.East;
            }
        }

        public static IntVector2 GetIntVector2(this PortalDirection portalDirection)
        {
            switch (portalDirection)
            {
                default:
                case PortalDirection.North: return IntVector2.up;
                case PortalDirection.East: return IntVector2.right;
                case PortalDirection.South: return IntVector2.down;
                case PortalDirection.West: return IntVector2.left;
            }
        }

        public static IntVector2 GetIntVector2(this DoorDirection portalDirection)
        {
            switch (portalDirection)
            {
                default:
                case DoorDirection.North: return IntVector2.up;
                case DoorDirection.East: return IntVector2.right;
                case DoorDirection.South: return IntVector2.down;
                case DoorDirection.West: return IntVector2.left;
            }
        }
    }

    public class RoomDungeon : Dungeon
    {
        public List<Room> rooms;

        public RoomDungeon(DungeonParameters parameters, Tile[,] tiles, int[,] regions, List<Room> rooms)
        {
            SetDungeonParameters(parameters);
            this.tiles = tiles;
            this.regions = regions;
            this.rooms = rooms;
        }

        /// <summary>
        /// Flips the dungeon along the horizontal axis (changes x values).
        /// </summary>
        public void FlipHorizontal()
        {
            Mirror(true);
        }

        /// <summary>
        /// Flips the dungeon along the vertical axis (changes y values).
        /// </summary>	
        public void FlipVertical()
        {
            Mirror(false);
        }


        private void Mirror(bool horizontal)
        {
            Debug.Log("WARNING, MIRROR FUNCTIONS STILL HAVE BUGS");
            int xSize = tiles.GetLength(0);
            int ySize = tiles.GetLength(1);

            MirrorTileArray(tiles, horizontal);

            foreach (Room room in rooms)
            {
                //mirror Tiles
                MirrorTileArray(room.tiles, horizontal);
                if (horizontal)
                {
                    room.x = xSize - 1 - room.x;
                }
                else
                {
                    room.y = ySize - 1 - room.y;
                }

                foreach (Door door in room.potentialDoors)
                {
                    if (horizontal)
                    {
                        door.x = xSize - 1 - door.x;
                        if (door.doorDir == DoorDirection.East)
                        {
                            door.doorDir = DoorDirection.West;
                        }
                        else if (door.doorDir == DoorDirection.West)
                        {
                            door.doorDir = DoorDirection.East;
                        }
                    }
                    else
                    {
                        door.y = ySize - 1 - door.y;
                        if (door.doorDir == DoorDirection.North)
                        {
                            door.doorDir = DoorDirection.South;
                        }
                        else if (door.doorDir == DoorDirection.South)
                        {
                            door.doorDir = DoorDirection.North;
                        }
                    }
                }

                foreach (Door door in room.doors)
                {
                    if (horizontal)
                    {
                        door.x = xSize - 1 - door.x;
                        if (door.doorDir == DoorDirection.East)
                        {
                            door.doorDir = DoorDirection.West;
                        }
                        else if (door.doorDir == DoorDirection.West)
                        {
                            door.doorDir = DoorDirection.East;
                        }
                    }
                    else
                    {
                        door.y = ySize - 1 - door.y;
                        if (door.doorDir == DoorDirection.North)
                        {
                            door.doorDir = DoorDirection.South;
                        }
                        else if (door.doorDir == DoorDirection.South)
                        {
                            door.doorDir = DoorDirection.North;
                        }
                    }
                }

                foreach (IntVector2 v2 in room.spawners)
                {
                    if (horizontal)
                    {
                        v2.x = xSize - 1 - v2.x;
                    }
                    else
                    {
                        v2.y = ySize - 1 - v2.y;
                    }
                }
            }
        }

        public void MirrorTileArray(Tile[,] array, bool horizontal)
        {
            int xSize = array.GetLength(0);
            int ySize = array.GetLength(1);

            Tile[,] newArray = new Tile[xSize, ySize];

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (horizontal)
                    {
                        newArray[xSize - 1 - x, y] = array[x, y];
                    }
                    else
                    {
                        newArray[x, ySize - 1 - y] = array[x, y];
                    }
                }
            }

            array = newArray;
        }
    }


//template of a room
    public class RoomTemplate /*, System.IComparable<RoomTemplate>*/
    {
        public string name;

        public Tile[,] tiles; //Tilegrid of the room
        public List<Door> potentialDoors;

        public List<IntVector2> spawners;

        public int width
        {
            //rect width
            get { return tiles.GetLength(0); }
        }

        public int height
        {
            //rect height
            get { return tiles.GetLength(1); }
        }

        //size Compare
        /*int System.IComparable<Room>.CompareTo(Room other){
        if (other.x*other.y > this.x*this.y)			return -1;
        else if (other.x*other.y == this.x*this.y)	return 0;
        else 										return 1;
    }*/

        public RoomTemplate()
        {
        }

        public RoomTemplate(Tile[,] tiles, List<Door> potentialDoors)
        {
            this.tiles = tiles;
            this.potentialDoors = potentialDoors;
        }
    }

//Room instance that has been placed, compared to the template it has helper variables for its state and graph
    public class Room : RoomTemplate
    {
        //State		
        public bool locked;


        public int x; //left line after placement
        public int y; //bottom line after placement		

        public IntVector2 GetOrigin
        {
            get { return new IntVector2(x, y); }
        }


        public PathRole pathrole = PathRole.Mandatory;

        //connected Rooms, not used for Graph, Graph consists out of doors
        public List<Room> connected = new List<Room>();
        public List<Door> doors = new List<Door>(); //doors that have been created when room is connected

        public int index; //index of room in List, assigned after it has been placed (startinb with zero)

        public Room()
        {
        }

        public Room(Tile[,] tiles, int x, int y, List<Door> potentialDoors)
        {
            this.tiles = tiles;
            this.x = x;
            this.y = y;
            this.potentialDoors = potentialDoors;
        }

        //move Door from potential to actual
        public void MakeDoorActual(Door potentialDoor)
        {
            this.doors.Add(potentialDoor);
            this.potentialDoors.Remove(potentialDoor);
        }
    }

//mandatory rooms lie on the solution path
//entrance exit is ragading the generation order, entrance is the first room placed, exit is te last
    public enum PathRole
    {
        Mandatory,
        Sidetrack,
        Hidden,
        Entrance,
        Exit
    }


    public class Door : System.IComparable<Door>
    {
        public DoorDirection doorDir; //direction of door
        public int doorLength; //length of door in Tiles
        public int x; //left line after placement (relative to room origin)
        public int y; //bottom line after placement (relative to room origin)

        public Room owner;

        //Graph helper for performing a search
        //public bool active		= false;	//can be passed, not used yet
        public bool visited = false; //visited
        public bool inList = false; //added to list
        public int distance = 10000;
        public Door prev;
        public List<Edge> connections = new List<Edge>();

        //distance Compare, needed to get Min in search
        int System.IComparable<Door>.CompareTo(Door other)
        {
            //use with .Equals()!
            if (other.distance > this.distance) return -1;
            else if (other.distance == this.distance) return 0;
            else return 1;
        }

        public Door(DoorDirection dir, int length, int x, int y)
        {
            this.doorDir = dir;
            this.doorLength = length;
            this.x = x;
            this.y = y;
        }

        public Door(DoorDirection dir, int length, int x, int y, Room owner)
        {
            this.doorDir = dir;
            this.doorLength = length;
            this.x = x;
            this.y = y;
            this.owner = owner;
        }

        /// <summary>
        /// Gets the coordinates of the tiles this door occupies (relative to room origin)
        /// </summary>
        /// <value>
        /// The occupied tiles coordinates.
        /// </value>
        public List<IntVector2> GetOccupiedTilesCoordinates
        {
            get
            {
                List<IntVector2> list = new List<IntVector2>();

                //initial Point
                list.Add(new IntVector2(x, y));

                //other Points
                if (doorDir == DoorDirection.East || doorDir == DoorDirection.West)
                {
                    for (int i = 1; i < doorLength; i++)
                    {
                        list.Add(new IntVector2(x, y + i));
                    }
                }
                else
                {
                    for (int i = 1; i < doorLength; i++)
                    {
                        list.Add(new IntVector2(x + i, y));
                    }
                }

                return list;
            }
        }
    }

//Edge, connection between two doors, length is one for doors that are placed to each other
    public class Edge
    {
        public Door node2;
        public int length;

        public Edge()
        {
        }

        public Edge(Door d2, int l)
        {
            this.node2 = d2;
            this.length = l;
        }
    }

    public enum DoorDirection
    {
        North,
        East,
        South,
        West
    };

    public class RoomTemplateDungeonPath
    {
        public List<Door> doors = new List<Door>();
        public List<Room> rooms = new List<Room>();
        public int length = 0;
    }

    #endregion

    #region DATATYPES

    public class IntVector2
    {
        public int x;
        public int y;

        public static IntVector2 up = new IntVector2(0, 1); //north
        public static IntVector2 right = new IntVector2(1, 0); //east
        public static IntVector2 down = new IntVector2(0, -1); //south
        public static IntVector2 left = new IntVector2(-1, 0); //west

        public static IntVector2 zero = new IntVector2(0, 0);

        public IntVector2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int sqrMagnitude
        {
            get { return x * x + y * y; }
        }

        //Casts
        public static implicit operator Vector2(IntVector2 From)
        {
            return new Vector2(From.x, From.y);
        }

        public static implicit operator IntVector2(Vector2 From)
        {
            return new IntVector2((int) From.x, (int) From.y);
        }

        public static implicit operator string(IntVector2 From)
        {
            return "(" + From.x + ", " + From.y + ")";
        }

        //Operators
        public static IntVector2 operator +(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }

        public static IntVector2 operator +(IntVector2 a, Vector2 b)
        {
            return new IntVector2(a.x + (int) b.x, a.y + (int) b.y);
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b)
        {
            return new IntVector2(a.x - b.x, a.y - b.y);
        }

        public static IntVector2 operator -(IntVector2 a, Vector2 b)
        {
            return new IntVector2(a.x - (int) b.x, a.y - (int) b.y);
        }

        public static IntVector2 operator *(IntVector2 a, int b)
        {
            return new IntVector2(a.x * b, a.y * b);
        }

        public static bool operator ==(IntVector2 iv1, IntVector2 iv2)
        {
            return (iv1.x == iv2.x && iv1.y == iv2.y);
        }

        public static bool operator !=(IntVector2 iv1, IntVector2 iv2)
        {
            return (iv1.x != iv2.x || iv1.y != iv2.y);
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            IntVector2 p = obj as IntVector2;
            if ((System.Object) p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        public bool Equals(IntVector2 p)
        {
            // If parameter is null return false:
            if ((object) p == null)
            {
                return false;
            }

            // Return true if the fields match:
            return (x == p.x) && (y == p.y);
        }

        public override int GetHashCode()
        {
            return x ^ y;
        }
    }

    public static class IntVector2Extensions
    {
        public static IntVector2 Clamp(this IntVector2 v2, IntVector2 min, IntVector2 max)
        {
            return new IntVector2(Mathf.Clamp(v2.x, min.x, max.x), Mathf.Clamp(v2.y, min.y, max.y));
        }
    }

/** Integer Rectangle.
	 * Works almost like UnityEngine.Rect but with integer coordinates , gathered from aarons pathfinding stuff
	 */
    public struct IntRect
    {
        public int xmin, ymin, xmax, ymax;

        public IntRect(int xmin, int ymin, int xmax, int ymax)
        {
            this.xmin = xmin;
            this.xmax = xmax;
            this.ymin = ymin;
            this.ymax = ymax;
        }

        public bool Contains(int x, int y)
        {
            return !(x < xmin || y < ymin || x > xmax || y > ymax);
        }

        public int Width
        {
            get { return xmax - xmin + 1; }
        }

        public int Height
        {
            get { return ymax - ymin + 1; }
        }

        /** Returns if this rectangle is valid.
     * An invalid rect could have e.g xmin > xmax.
     * Rectamgles with a zero area area invalid.
     */
        public bool IsValid()
        {
            return xmin <= xmax && ymin <= ymax;
        }

        public static bool operator ==(IntRect a, IntRect b)
        {
            return a.xmin == b.xmin && a.xmax == b.xmax && a.ymin == b.ymin && a.ymax == b.ymax;
        }

        public static bool operator !=(IntRect a, IntRect b)
        {
            return a.xmin != b.xmin || a.xmax != b.xmax || a.ymin != b.ymin || a.ymax != b.ymax;
        }

        public override bool Equals(System.Object _b)
        {
            IntRect b = (IntRect) _b;
            return xmin == b.xmin && xmax == b.xmax && ymin == b.ymin && ymax == b.ymax;
        }

        public override int GetHashCode()
        {
            return xmin * 131071 ^ xmax * 3571 ^ ymin * 3109 ^ ymax * 7;
        }

        /** Returns the intersection rect between the two rects.
     * The intersection rect is the area which is inside both rects.
     * If the rects do not have an intersection, an invalid rect is returned.
     * \see IsValid
     */
        public static IntRect Intersection(IntRect a, IntRect b)
        {
            IntRect r = new IntRect(
                System.Math.Max(a.xmin, b.xmin),
                System.Math.Max(a.ymin, b.ymin),
                System.Math.Min(a.xmax, b.xmax),
                System.Math.Min(a.ymax, b.ymax)
            );

            return r;
        }

        /** Returns if the two rectangles intersect each other
     */
        public static bool Intersects(IntRect a, IntRect b)
        {
            return !(a.xmin > b.xmax || a.ymin > b.ymax || a.xmax < b.xmin || a.ymax < b.ymin);
        }

        /** Returns a new rect which contains both input rects.
     * This rectangle may contain areas outside both input rects as well in some cases.
     */
        public static IntRect Union(IntRect a, IntRect b)
        {
            IntRect r = new IntRect(
                System.Math.Min(a.xmin, b.xmin),
                System.Math.Min(a.ymin, b.ymin),
                System.Math.Max(a.xmax, b.xmax),
                System.Math.Max(a.ymax, b.ymax)
            );

            return r;
        }

        /** Returns a new IntRect which is expanded to contain the point */
        public IntRect ExpandToContain(int x, int y)
        {
            IntRect r = new IntRect(
                System.Math.Min(xmin, x),
                System.Math.Min(ymin, y),
                System.Math.Max(xmax, x),
                System.Math.Max(ymax, y)
            );
            return r;
        }

        /** Returns a new rect which is expanded by \a range in all directions.
     * \param range How far to expand. Negative values are permitted.
     */
        public IntRect Expand(int range)
        {
            return new IntRect(xmin - range,
                ymin - range,
                xmax + range,
                ymax + range
            );
        }

        /** Matrices for rotation.
     * Each group of 4 elements is a 2x2 matrix.
     * The XZ position is multiplied by this.
     * So
     * \code
     * //A rotation by 90 degrees clockwise, second matrix in the array
     * (5,2) * ((0, 1), (-1, 0)) = (2,-5)
     * \endcode
     */
        private static readonly int[] Rotations =
        {
            1, 0, //Identity matrix
            0, 1,

            0, 1,
            -1, 0,

            -1, 0,
            0, -1,

            0, -1,
            1, 0
        };

        /** Returns a new rect rotated around the origin 90*r degrees.
     * Ensures that a valid rect is returned.
     */
        public IntRect Rotate(int r)
        {
            int mx1 = Rotations[r * 4 + 0];
            int mx2 = Rotations[r * 4 + 1];
            int my1 = Rotations[r * 4 + 2];
            int my2 = Rotations[r * 4 + 3];

            int p1x = mx1 * xmin + mx2 * ymin;
            int p1y = my1 * xmin + my2 * ymin;

            int p2x = mx1 * xmax + mx2 * ymax;
            int p2y = my1 * xmax + my2 * ymax;

            return new IntRect(
                System.Math.Min(p1x, p2x),
                System.Math.Min(p1y, p2y),
                System.Math.Max(p1x, p2x),
                System.Math.Max(p1y, p2y)
            );
        }

        /** Returns a new rect which is offset by the specified amount.
     */
        public IntRect Offset(Int2 offset)
        {
            return new IntRect(xmin + offset.x, ymin + offset.y, xmax + offset.x, ymax + offset.y);
        }

        /** Returns a new rect which is offset by the specified amount.
     */
        public IntRect Offset(int x, int y)
        {
            return new IntRect(xmin + x, ymin + y, xmax + x, ymax + y);
        }

        public override string ToString()
        {
            return "[x: " + xmin + "..." + xmax + ", y: " + ymin + "..." + ymax + "]";
        }

        /** Draws some debug lines representing the rect */
        public void DebugDraw(Matrix4x4 matrix, Color col)
        {
            Vector3 p1 = matrix.MultiplyPoint3x4(new Vector3(xmin, 0, ymin));
            Vector3 p2 = matrix.MultiplyPoint3x4(new Vector3(xmin, 0, ymax));
            Vector3 p3 = matrix.MultiplyPoint3x4(new Vector3(xmax, 0, ymax));
            Vector3 p4 = matrix.MultiplyPoint3x4(new Vector3(xmax, 0, ymin));

            Debug.DrawLine(p1, p2, col);
            Debug.DrawLine(p2, p3, col);
            Debug.DrawLine(p3, p4, col);
            Debug.DrawLine(p4, p1, col);
        }
    }

/** Two Dimensional Integer Coordinate Pair , gathered from aarons pathfinding stuff*/
    public struct Int2
    {
        public int x;
        public int y;

        public Int2(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public int sqrMagnitude
        {
            get { return x * x + y * y; }
        }

        public long sqrMagnitudeLong
        {
            get { return (long) x * (long) x + (long) y * (long) y; }
        }

        public static Int2 operator +(Int2 a, Int2 b)
        {
            return new Int2(a.x + b.x, a.y + b.y);
        }

        public static Int2 operator -(Int2 a, Int2 b)
        {
            return new Int2(a.x - b.x, a.y - b.y);
        }

        public static bool operator ==(Int2 a, Int2 b)
        {
            return a.x == b.x && a.y == b.y;
        }

        public static bool operator !=(Int2 a, Int2 b)
        {
            return a.x != b.x || a.y != b.y;
        }

        public static int Dot(Int2 a, Int2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        public static long DotLong(Int2 a, Int2 b)
        {
            return (long) a.x * (long) b.x + (long) a.y * (long) b.y;
        }

        public override bool Equals(System.Object o)
        {
            if (o == null) return false;
            Int2 rhs = (Int2) o;

            return x == rhs.x && y == rhs.y;
        }

        public override int GetHashCode()
        {
            return x * 49157 + y * 98317;
        }

        /** Matrices for rotation.
     * Each group of 4 elements is a 2x2 matrix.
     * The XZ position is multiplied by this.
     * So
     * \code
     * //A rotation by 90 degrees clockwise, second matrix in the array
     * (5,2) * ((0, 1), (-1, 0)) = (2,-5)
     * \endcode
     */
        private static readonly int[] Rotations =
        {
            1, 0, //Identity matrix
            0, 1,

            0, 1,
            -1, 0,

            -1, 0,
            0, -1,

            0, -1,
            1, 0
        };

        /** Returns a new Int2 rotated 90*r degrees around the origin. */
        public static Int2 Rotate(Int2 v, int r)
        {
            r = r % 4;
            return new Int2(v.x * Rotations[r * 4 + 0] + v.y * Rotations[r * 4 + 1],
                v.x * Rotations[r * 4 + 2] + v.y * Rotations[r * 4 + 3]);
        }

        public static Int2 Min(Int2 a, Int2 b)
        {
            return new Int2(System.Math.Min(a.x, b.x), System.Math.Min(a.y, b.y));
        }

        public static Int2 Max(Int2 a, Int2 b)
        {
            return new Int2(System.Math.Max(a.x, b.x), System.Math.Max(a.y, b.y));
        }

        /*public static Int2 FromInt3XZ (Int3 o) {
        return new Int2 (o.x,o.z);
    }*/

        /*public static Int3 ToInt3XZ (Int2 o) {
        return new Int3 (o.x,0,o.y);
    }*/

        public override string ToString()
        {
            return "(" + x + ", " + y + ")";
        }
    }

    #endregion
}