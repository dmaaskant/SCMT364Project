using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCMT364Project
{
    internal class Task
    {
        private string name;
        private double time;
        private List<string> parents;
        private List<string> followers;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Task(string name, double time, List<string> parents, List<string> followers)
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        {
            this.Name = name;
            this.Time = time;
            this.Parents = parents;
            this.followers = followers;
        }
        public Task()
        {
            name = "Q";
            time = -1;
            parents = new List<string>();
            followers = new List<string>();
        }
        public string Name { get => name; set => name = value; }
        public double Time { get => time; set => time = value; }
        public List<string> Parents { get => parents; set => parents = value; }
        public List<string> Followers { get => followers; set => followers = value; }

    }
}
