using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace OpenlabsLauncher.ConsoleUI
{
    internal class ConsoleOption
    {
        public void SetSelected(bool selected)
            => Selected = selected;

        public string name;
        public Action action;

        public bool Selected { get; private set; }
    }

    internal class ConsoleOptionView
    {
        public ConsoleOptionView(string title, ConsoleOption[] options)
        {
            _title = title;
            _teardown = false;
            _options = options;

            _selectedIdx = 0;
            _isDirty = true;
        }

        private Dictionary<string, ConsoleOption> _reverseLookup
        {
            get
            {
                var temp = new Dictionary<string, ConsoleOption>();

                foreach (var option in _options)
                    temp.Add(option.name, option);

                return temp;
            }
        }

        private Dictionary<int, ConsoleOption> _optionIdxLookup
        {
            get
            {
                var temp = new Dictionary<int, ConsoleOption>();

                int idx = 0;
                foreach (var option in _options)
                {
                    temp.Add(idx, option);
                    idx++;
                }

                return temp;
            }
        }

        private void UpdateSelected()
        {
            var keyInfo = Console.ReadKey();

            if (keyInfo.Key == ConsoleKey.UpArrow ||
                keyInfo.Key == ConsoleKey.W)
            {
                _selectedIdx = Math.Clamp(_selectedIdx - 1, 0, _options.Length - 1);
                _isDirty = true;
            }

            if (keyInfo.Key == ConsoleKey.DownArrow ||
                keyInfo.Key == ConsoleKey.S)
            {
                _selectedIdx = Math.Clamp(_selectedIdx + 1, 0, _options.Length - 1);
                _isDirty = true;
            }

            if (keyInfo.Key == ConsoleKey.Enter ||
                keyInfo.Key == ConsoleKey.W)
            {
                _options[_selectedIdx].action();
                _isDirty = true;
            }

            foreach (var option in _optionIdxLookup)
            {
                _options[option.Key].SetSelected(option.Key == _selectedIdx);
            }
        }

        private void UpdateDisplay()
        {
            #region Logic

            if (_finishedTeardown) return;
            if (_teardown)
            {
                Console.Clear();
                _finishedTeardown = true;
                return;
            }

            if (_firstRun)
            {
                _selectedIdx = 0;
                _isDirty = true;
                _firstRun = false;
            }

            if (!_isDirty) return;
            _isDirty = false;

            #endregion
            Console.Clear();

            Console.WriteLine(_title);
            foreach (var option in _optionIdxLookup)
            {
                if (!_options[option.Key].Selected)
                {
                    Console.ForegroundColor = ConsoleColor.Gray;
                    Console.WriteLine($"  {option.Value.name}");
                    Console.ResetColor();
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"-  {option.Value.name}");
                    Console.ResetColor();
                }
            }
        }

        public void Update()
        {
            UpdateSelected();
            UpdateDisplay();
        }

        public void Teardown()
        { _teardown = true; }

        public void Invoke(int optionIdx)
            => _options[optionIdx].action();

        private Task _task;

        private string _title;
        private ConsoleOption[] _options;
        private bool _teardown;
        private bool _finishedTeardown;
        private int _selectedIdx;
        private bool _isDirty;
        private bool _firstRun = true;
    }
}
