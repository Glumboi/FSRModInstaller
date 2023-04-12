using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FSRModInstaller.Xml;

public struct Game
{
    public string _name;

    public string Name
    {
        get => _name;
        set
        {
            if (value == _name) return;
            _name = value;
        }
    }

    public string _bin;

    public string Bin
    {
        get => _bin;
        set
        {
            if (value == _bin) return;
            _bin = value;
        }
    }

    public Game(string name, string bin)
    {
        _name = name;
        _bin = bin;
    }
}