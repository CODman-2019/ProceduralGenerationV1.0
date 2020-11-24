using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[Flags] // used to determine if a wall is 
public enum WallState
{
    //0000 ->
    //0001    0010       0100    1000
    LEFT = 1, RIGHT = 2, UP = 4, DOWN= 8,
    //1000 0000
    VISITED = 128,

    //player position
    PLAYER = 16, //10000
    //teleport position
    TELEPORTER = 32
}

public struct Position
{
    public int x;
    public int y;
}

public struct Neighbour
{
    public Position Position;
    public WallState SharedWall;
}


public static class MazeGenerator 
{

    private static WallState GetOppositeWall(WallState wall)
    {
        switch (wall)
        {
            case WallState.RIGHT: return WallState.LEFT;
            case WallState.LEFT: return WallState.RIGHT;
            case WallState.UP: return WallState.DOWN;
            case WallState.DOWN: return WallState.UP;
            default: return WallState.VISITED;
        }

    }

    //build maze itself
    private static WallState[,] ApplyRecursiveBacktracker(WallState[,] maze, int width, int height, bool playerPlacement)
    {
        var rng = new System.Random(/*SEED*/);

        bool teleportPlaced = false;
        //create a stack variable
        var positionStack = new Stack<Position>();


        //pic a random point in the maze
        var position = new Position { x = rng.Next(0, width), y = rng.Next(0, height)};

        //set it spot as a visited state
        maze[position.x, position.y] |= WallState.VISITED;

        //add player state if the player placement is true;
        if (playerPlacement)
        {
            maze[position.x, position.y] |= WallState.PLAYER;
        }

        positionStack.Push(position);

        //go through the maze 
        while(positionStack.Count > 0)
        {
            //remove the current position from the stack
            var current = positionStack.Pop();
            // get a list of neighbours
            var neighbours = GetUnvisitedNeighbours(current, maze, width, height);


            if(neighbours.Count > 0)
            {
                positionStack.Push(current);

                var randIndex = rng.Next(0, neighbours.Count);
                var randomNeighbour = neighbours[randIndex];

                //get the neighbours position and remove the shared wall from both sides
                var nPosition = randomNeighbour.Position;
                maze[current.x, current.y] &= ~randomNeighbour.SharedWall; // remove the shared wall
                maze[nPosition.x, nPosition.y] &= ~GetOppositeWall(randomNeighbour.SharedWall); // remove neighbours shared wall

                //mark space as visited
                maze[nPosition.x, nPosition.y] |= WallState.VISITED;

                //push into the stack
                positionStack.Push(nPosition);
            }
            else
            {
                if (!teleportPlaced)
                {
                    maze[current.x, current.y] |= WallState.TELEPORTER;
                    teleportPlaced = false;
                }
                
            }
        }


        return maze;
    }

    //return a list of nieighbours that were not visited
    private static List<Neighbour> GetUnvisitedNeighbours(Position p, WallState[,] maze, int width, int height)
    {
        var list = new List<Neighbour>();

        if(p.x > 0) // left
        {
            //if the node is not visited
            if(!maze[p.x - 1, p.y].HasFlag(WallState.VISITED))
            {
                //add neighbour to list
                list.Add(new Neighbour
                {
                    Position = new Position
                    {
                        x = p.x - 1,
                        y = p.y
                    },
                    SharedWall = WallState.LEFT
                });
            }
        }

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

    //create empty grid
    public static WallState[,] Generate(int width, int height, bool playerPlacement)
    {
        WallState[,] maze = new WallState[width, height];
        WallState startState = WallState.RIGHT | WallState.LEFT | WallState.UP | WallState.DOWN;

        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y++)
            {
                maze[x, y] = startState; // 1111

            }
        }



        return ApplyRecursiveBacktracker(maze, width, height, playerPlacement);

    }
}
