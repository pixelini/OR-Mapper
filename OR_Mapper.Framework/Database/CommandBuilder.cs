using System;
using System.Collections.Generic;
using System.Threading;

namespace OR_Mapper.Framework.Database
{
    public class CommandBuilder
    {
        private string _table;
        private List<string> _select;
        private string _where;
        private string _commandString;
        private List<string> _selectCommandClauses = new();

        public CommandBuilder()
        {
            _selectCommandClauses.Add("where");
        }
        
        public string Table(string table)
        {
            _table = table;
            return table;
        }
        
        public List<string> Select(List<string> select)
        {
            return select;
        }

        private string buildSelectCommand()
        {
            var command = "";
            if (_select.Count == 0)
            {
                command = "" + _table + ".*"; // takes every single column from the table
            }
            else
            {
                command = string.Join(',', _select); // separates all selected columns by comma
            }
            
            command = "select " + command + " from " + _table + " ";

            // append clauses
            foreach (var clause in _selectCommandClauses)
            {
                command += (clause + ' ');
            }
            
            // removes the last whitespace from the string
            command = command.Substring(0, command.Length - command.LastIndexOf(' '));
            _commandString = command;
            return _commandString;
        }

        public string get()
        {
            return buildSelectCommand();
        }
    }
}