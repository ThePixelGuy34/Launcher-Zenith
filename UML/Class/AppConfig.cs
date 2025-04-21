using System;
using System.Collections.Generic;
using System.Text;

namespace UML.Class.DontBreakPls
{
    public class AppConfig
    {
        public List<GameConfig> games { get; set; }
        public MainContent? Main { get; set; }
        public List<ImageOnlyConfig> Images { get; set; }
    }

    public class GameConfig
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public List<ImageConfig> images { get; set; }
    }

    public class Game
    {
        public string? id { get; set; }
        public string? name { get; set; }
        public List<ImageData> images { get; set; }
    }

    public class ImageConfig
    {
        public string? id { get; set; }
        public string? image { get; set; }
        public string? name { get; set; }
        public string? header { get; set; }
        public string? description { get; set; }
        public string? buttontxt { get; set; }
    }

    public class ImageData
    {
        public string? id { get; set; }
        public string? image { get; set; }
        public string? Name { get; set; }
    }

    public class MainContent
    {
        public string? video { get; set; }
    }

    public class ImageOnlyConfig
    {
        public string? image { get; set; }
    }
}

