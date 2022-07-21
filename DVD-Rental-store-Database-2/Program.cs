using System;
using System.ComponentModel.Design;
using Npgsql;
using Microsoft.Extensions.Configuration;
namespace Database
{
    class Program
    {
        
        static void Main(string[] args)
        {
            new Menu().Begin();
        }
    }
}
