﻿using System;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoUtility.Basic;

namespace MongoGUICtl
{
    public partial class ctlBsonValue : UserControl
    {
        /// <summary>
        ///     数组
        /// </summary>
        private BsonArray _mBsonArray = new BsonArray();
        /// <summary>
        ///     获得一个新BsonArray的委托
        /// </summary>
        public static Func<BsonArray> GetArray;

        /// <summary>
        ///     获得一个新BsonArray的委托
        /// </summary>
        public static Func<BsonArray> GetGeo;

        /// <summary>
        ///     文档
        /// </summary>
        private BsonDocument _mBsonDocument = new BsonDocument();
        /// <summary>
        ///     获得一个新BsonDocument的委托
        /// </summary>
        public static Func<BsonDocument> GetDocument;

        /// <summary>
        ///     初始化，请确保 getArray 和 getDocument正确设定
        /// </summary>
        public ctlBsonValue()
        {
            InitializeComponent();

            dateTimePicker.Location = txtBsonValue.Location;
            dateTimePicker.Size = txtBsonValue.Size;
            radTrue.Location = txtBsonValue.Location;
            radFalse.Top = txtBsonValue.Top;
            NumberPick.Location = txtBsonValue.Location;
            NumberPick.Size = txtBsonValue.Size;
            NumberPick.Minimum = int.MinValue;
            NumberPick.Maximum = int.MaxValue;

            txtBsonValue.Visible = true;
            txtBsonValue.Text = string.Empty;
            radTrue.Visible = false;
            radFalse.Visible = false;
            radFalse.Checked = true;
            dateTimePicker.Visible = false;
            NumberPick.Visible = false;
        }

        /// <summary>
        ///     使用属性会发生一些MONO上的移植问题
        /// </summary>
        /// <returns></returns>
        public BsonValue GetValue(BsonValueEx.BasicType DataType)
        {
            BsonValue mValue = null;
            switch (DataType)
            {
                case BsonValueEx.BasicType.BsonString:
                    mValue = new BsonString(txtBsonValue.Text);
                    break;
                case BsonValueEx.BasicType.BsonInt32:
                    mValue = new BsonInt32(Convert.ToInt32(NumberPick.Value));
                    break;
                case BsonValueEx.BasicType.BsonInt64:
                    mValue = new BsonInt64(Convert.ToInt64(NumberPick.Value));
                    break;
                case BsonValueEx.BasicType.BsonDecimal128:
                    mValue = new BsonDecimal128(Convert.ToDecimal(NumberPick.Value));
                    break;
                case BsonValueEx.BasicType.BsonDouble:
                    mValue = new BsonDouble(Convert.ToDouble(txtBsonValue.Text));
                    break;
                case BsonValueEx.BasicType.BsonDateTime:
                    mValue = new BsonDateTime(dateTimePicker.Value);
                    break;
                case BsonValueEx.BasicType.BsonBoolean:
                    mValue = radTrue.Checked ? BsonBoolean.True : BsonBoolean.False;
                    break;
                case BsonValueEx.BasicType.BsonArray:
                case BsonValueEx.BasicType.BsonGeo:
                    mValue = _mBsonArray;
                    break;
                case BsonValueEx.BasicType.BsonDocument:
                    mValue = _mBsonDocument;
                    break;
            }
            return mValue;
        }

        /// <summary>
        ///     使用属性会发生一些MONO上的移植问题
        /// </summary>
        /// <returns></returns>
        public void SetValue(BsonValue value, BsonValueEx.BasicType DataType = BsonValueEx.BasicType.BsonUndefined)
        {
            txtBsonValue.Visible = false;
            txtBsonValue.Text = string.Empty;
            txtBsonValue.ReadOnly = false;
            radTrue.Visible = false;
            radFalse.Visible = false;
            radFalse.Checked = true;
            dateTimePicker.Visible = false;
            NumberPick.Visible = false;

            if (value.IsString)
            {
                txtBsonValue.Visible = true;
                txtBsonValue.Text = value.ToString();
            }

            if (value.IsInt32)
            {
                NumberPick.Visible = true;
                NumberPick.Value = value.AsInt32;
            }

            if (value.IsInt64)
            {
                NumberPick.Visible = true;
                NumberPick.Value = value.AsInt64;
            }

            if (value.IsDecimal128)
            {
                NumberPick.Visible = true;
                NumberPick.Value = value.AsDecimal;
            }

            if (value.IsDouble)
            {
                txtBsonValue.Visible = true;
                txtBsonValue.Text = value.AsDouble.ToString();
            }

            if (value.IsValidDateTime)
            {
                dateTimePicker.Visible = true;
                dateTimePicker.Value = value.ToUniversalTime();
            }

            if (value.IsBoolean)
            {
                radTrue.Visible = true;
                radFalse.Visible = true;
                if (value.AsBoolean)
                {
                    radTrue.Checked = true;
                }
                else
                {
                    radFalse.Checked = true;
                }
            }
            if (value.IsBsonArray)
            {
                if (GetArray == null)
                {
                    MessageBox.Show("GetArray委托不存在！");
                    return;
                }
                if (DataType == BsonValueEx.BasicType.BsonGeo)
                {
                    //地理
                    var t = GetGeo();
                    if (t != null)
                    {
                        _mBsonArray = t;
                        txtBsonValue.Visible = true;
                        txtBsonValue.Text = _mBsonArray.ToString();
                        txtBsonValue.ReadOnly = true;
                    }
                }
                else
                {
                    //普通数组
                    var t = GetArray();
                    if (t != null)
                    {
                        _mBsonArray = t;
                        txtBsonValue.Visible = true;
                        txtBsonValue.Text = _mBsonArray.ToString();
                        txtBsonValue.ReadOnly = true;
                    }
                }

            }
            if (value.IsBsonDocument)
            {
                if (GetDocument == null)
                {
                    MessageBox.Show("GetDocument委托不存在！");
                    return;
                }
                var t = GetDocument();
                if (t != null)
                {
                    _mBsonDocument = t;
                    txtBsonValue.Visible = true;
                    txtBsonValue.Text = _mBsonDocument.ToString();
                    txtBsonValue.ReadOnly = true;
                }
            }
        }

        /// <summary>
        ///     
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DataTypeChanged(BsonValueEx.BasicType DataType)
        {
            txtBsonValue.Visible = false;
            radTrue.Visible = false;
            radFalse.Visible = false;
            SetValue(BsonValueEx.GetInitValue(DataType), DataType);
        }
    }
}