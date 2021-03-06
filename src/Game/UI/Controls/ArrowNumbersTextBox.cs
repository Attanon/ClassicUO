﻿#region license

// Copyright (C) 2020 ClassicUO Development Community on Github
// 
// This project is an alternative client for the game Ultima Online.
// The goal of this is to develop a lightweight client considering
// new technologies.
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.

#endregion

using System;
using ClassicUO.Renderer;

namespace ClassicUO.Game.UI.Controls
{
    internal class ArrowNumbersTextBox : Control
    {
        private const int TIME_BETWEEN_CLICKS = 250;
        private readonly int _Min, _Max;
        private readonly StbTextBox _textBox;
        private float _timeUntilNextClick;
        private readonly Button _up, _down;

        public ArrowNumbersTextBox
        (
            int x,
            int y,
            int width,
            int raiseamount,
            int minvalue,
            int maxvalue,
            byte font = 0,
            int maxcharlength = -1,
            bool isunicode = true,
            FontStyle style = FontStyle.None,
            ushort hue = 0
        )
        {
            int height = 20;
            X = x;
            Y = y;
            Width = width;
            Height = height;
            _Min = minvalue;
            _Max = maxvalue;

            Add
            (
                new ResizePic(0x0BB8)
                {
                    Width = width,
                    Height = height + 4
                }
            );

            _up = new Button(raiseamount, 0x983, 0x984)
            {
                X = width - 12,
                ButtonAction = ButtonAction.Activate
            };

            _up.MouseDown += (sender, e) =>
            {
                if (_up.IsClicked)
                {
                    UpdateValue();
                    _timeUntilNextClick = TIME_BETWEEN_CLICKS * 2;
                }
            };

            Add(_up);

            _down = new Button(-raiseamount, 0x985, 0x986)
            {
                X = width - 12,
                Y = height - 7,
                ButtonAction = ButtonAction.Activate
            };

            _down.MouseDown += (sender, e) =>
            {
                if (_down.IsClicked)
                {
                    UpdateValue();
                    _timeUntilNextClick = TIME_BETWEEN_CLICKS * 2;
                }
            };

            Add(_down);

            Add
            (
                _textBox = new StbTextBox(font, maxcharlength, width, isunicode, style, hue)
                {
                    X = 2,
                    Y = 2,
                    Height = height,
                    Width = width - 17,
                    NumbersOnly = true
                }
            );
        }

        internal string Text
        {
            get => _textBox?.Text ?? string.Empty;
            set => _textBox?.SetText(value);
        }

        private void UpdateValue()
        {
            int.TryParse(_textBox.Text, out int i);

            if (_up.IsClicked)
            {
                i += _up.ButtonID;
            }
            else
            {
                i += _down.ButtonID;
            }

            ValidateValue(i);
        }

        internal override void OnFocusLost()
        {
            if (IsDisposed)
            {
                return;
            }

            int.TryParse(_textBox.Text, out int i);
            ValidateValue(i);
        }

        private void ValidateValue(int val)
        {
            Tag = val = Math.Max(_Min, Math.Min(_Max, val));
            _textBox.SetText(val.ToString());
        }

        public override void Update(double totalTime, double frameTime)
        {
            if (IsDisposed)
            {
                return;
            }

            if (_up.IsClicked || _down.IsClicked)
            {
                if (_timeUntilNextClick <= 0f)
                {
                    _timeUntilNextClick += TIME_BETWEEN_CLICKS;
                    UpdateValue();
                }

                _timeUntilNextClick -= (float) frameTime;
            }

            base.Update(totalTime, frameTime);
        }
    }
}