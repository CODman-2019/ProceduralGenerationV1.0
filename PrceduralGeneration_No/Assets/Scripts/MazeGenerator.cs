using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Flags] // used to determine if a wall is visited, what wall is there and if a player or item is placed
public enum WallState
{
    //based on binary value
    //0001    0010       0100    1000
    LEFT = 1, RIGHT = 2, UP = 4, DOWN= 8,
    VISITED = 128,
    PLAYER = 16,
    ITEM = 32

}

//struct for position
public struct Position
{
    public int x;
    public int y;
}

//struct for neighbour with a position variable and a Wallstate variable
public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}


public static class MazeGenerator 
{
    //method returns a wallstate based what wallState is taken in
    private static WallState GetOppositeWall(WallState wall)
    {
        //switch statement used to get the opposite wallstate
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            //return if the wallstate was already visited
            default: return WallState.VISITED;
        }

    }

    //build maze itself by going through and removing walls
    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height)
    {
        //random var
        var rng = new System.Random(/*SEED*/);

        //create a stack variable
        var positionStack = new Stack<Position>();

        //pic a random point in the maze
        var position = new Position { x = rng.Next(0, width), y = rng.Next(0, height)};

        //set it spot as a visited state and set player
        maze[position.x, position.y] |= WallState.VISITED;
        maze[position.x, position.y] |= WallState.PLAYER;

        //push the position to the stack of positions
        positionStack.Push(position);


        //go through the maze while the stack is not empty
        while(positionStack.Count > 0)
        {
            // get the current position and remove from the stack
            var current = positionStack.Pop();

            // get a list of surrounding neighbours not visited
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);

            //if the list of neighbours is not empty
            if(neighbours.Count > 0)
            {
                //push the current position
                positionStack.Push(current); 

                //randomly pick a neighbour from the neighbour list
                var randIndex = rng.Next(0, neighbours.Count);
                var randomNeighbour = neighbours[randIndex];

                //get the neighbours position and remove the shared wall from both sides
                var nPosition = randomNeighbour.Position;

                //removes the wall from both positions (current and neighbour)
                maze[current.x, current.y] &= ~randomNeighbour.SharedWall; // remove the shared wall
                maze[nPosition.x, nPosition.y] &= ~GetOppositeWall(randomNeighbour.SharedWall); // remove neighbours shared wall

                // randomly add an item to the cell
                int itemplace = rng.Next(0, 2);
                if (itemplace != 0 && !maze[current.x, current.y].HasFlag(WallState.PLAYER) ) maze[current.x, current.y] |= WallState.ITEM;

                //mark space as visited
                maze[nPosition.x, nPosition.y] |= WallState.VISITED;

                //push into the stack
                positionStack.Push(nPosition);
            }
        }

        //return the maze
        return maze;
    }

    //return a list of nieighbours that were not visited around the current position
    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, int width, int height)
    {
        var list = new List<Neighbour>();

        //first 2 are for the middle sections (not the end column or row)

        if(p.x > 0) // left
        {
            //if the node is not visited
            if(!maze[p.x - 1, p.y].HasFlag(WallState.VISITED))
            {
                //add neighbour to list
                list.Add(new Neighbour
                {
                    //create the position for the neighbour 
                    Position = new Position
                    {
                        x = p.x - 1,
                        y = p.y
                    },
                    //mark the shared wall
                    SharedWall = WallState.LEFT
                });
            }
        }
        
        //other if statements are similar but for different walls and different locations
        if (p.y > 0) // Bottom
        {
            if (!maze[p.x, p.y - 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        x = p.x,
                        y = p.y - 1
                    },
                    SharedWall = WallState.DOWN
                });
            }
        }

        //last if statements are for the last row and columns
        if (p.y < height - 1) // top
        {
            if (!maze[p.x, p.y + 1].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        x = p.x,
                        y = p.y + 1
                    },
                    SharedWall = WallState.UP
                });
            }
        }

        if (p.x < width - 1) // right
        {
            if (!maze[p.x + 1, p.y].HasFlag(WallState.VISITED))
            {
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        x = p.x + 1,
                        y = p.y
                    },
                    SharedWall = WallState.RIGHT
                });
            }
        }
        return list;
    }

    //creates maze
    public static WallState[,] Generate(int width, int height)
    {
        //create a new 2D array of wallstates with all the walls as wall states
        WallState[,] maze = new WallState[width, height];
        WallState startState = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;

        //nested for loop - mark each maze cell with the start state
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                maze[x, y] = startState; // 1111

            }
        }

        //use Recursion method to build maze
        return ApplyRecursiveBacktracker(maze, width, height);

    }
}
