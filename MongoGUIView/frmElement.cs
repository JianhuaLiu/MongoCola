﻿using System;
using System.Windows.Forms;
using MongoDB.Bson;
using MongoUtility.Command;
using MongoUtility.Core;
using ResourceLib.Method;
using ResourceLib.UI;
using MongoUtility.Basic;

namespace MongoGUIView
{
    public partial class FrmElement : Form
    {
        /// <summary>
        ///     路径
        /// </summary>
        private readonly string _fullPath = string.Empty;

        /// <summary>
        ///     是否为元素
        /// </summary>
        private readonly bool _isElement = true;

        /// <summary>
        ///     是否为更新模式
        /// </summary>
        private readonly bool _isUpdateMode;

        /// <summary>
        ///     选中的树节点
        /// </summary>
        private readonly TreeNode _selectNode;

        /// <summary>
        ///     是否加载
        /// </summary>
        private bool IsLoaded = false;

        /// <summary>
        /// </summary>
        /// <param name="isUpdateMode"></param>
        /// <param name="selectNode"></param>
        /// <param name="isElement"></param>
        public FrmElement(bool isUpdateMode, TreeNode selectNode, bool isElement)
        {
            InitializeComponent();
            _isUpdateMode = isUpdateMode;
            _fullPath = selectNode.FullPath;
            _selectNode = selectNode;
            _isElement = isElement;
        }

        /// <summary>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmElement_Load(object sender, EventArgs e)
        {
            Common.Utility.FillComberWithEnum(cmbDataType, typeof(BsonValueEx.BasicType), true);
            if (_isUpdateMode)
            {
                txtElName.Visible = true;
                txtElName.Enabled = false;
                lblElement.Visible = true;
                if (_selectNode.Tag.GetType() == typeof(BsonElement))
                {
                    var El = (BsonElement)_selectNode.Tag;
                    txtElName.Text = (El).Name;
                    ctlBsonValue1.SetValue(El.Value);
                    cmbDataType.SelectedIndex = (int)BsonValueEx.GetBsonValueBasicType(El.Value);
                }else
                {
                    var value = (BsonValue)_selectNode.Tag;
                    ctlBsonValue1.SetValue(value);
                    cmbDataType.SelectedIndex = (int)BsonValueEx.GetBsonValueBasicType(value);
                }
            }
            if (!_isElement)
            {
                //TODO:在这个模式，数组里面暂时不能添加数组或者文档
                txtElName.Visible = false;
                lblElement.Visible = false;
            }
            IsLoaded = true;
            if (!GuiConfig.IsUseDefaultLanguage)
            {
                cmdOK.Text = GuiConfig.GetText(TextType.CommonOk);
                cmdCancel.Text = GuiConfig.GetText(TextType.CommonCancel);
            }

        }

        /// <summary>
        ///     确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdOK_Click(object sender, EventArgs e)
        {

            var basictype = (BsonValueEx.BasicType)cmbDataType.SelectedIndex;
            var ElValue = ctlBsonValue1.GetValue(basictype);
            var ElName = txtElName.Text;
            var Element = new BsonElement(ElName, ElValue);
            if (_isUpdateMode)
            {
                if (_isElement)
                {
                    ElementHelper.ModifyElement(_fullPath, ElValue,
                        (BsonElement)_selectNode.Tag,
                        RuntimeMongoDbContext.CurrentDocument,
                        RuntimeMongoDbContext.GetCurrentCollection());
                    //元素的场合，Tag直接放入元素
                    _selectNode.Tag = Element;
                }
                else
                {
                    ElementHelper.ModifyArrayValue(_fullPath, ElValue, _selectNode.Index,
                        RuntimeMongoDbContext.CurrentDocument,
                        RuntimeMongoDbContext.GetCurrentCollection());
                    _selectNode.Tag = ElValue;
                }
                _selectNode.Text = string.IsNullOrEmpty(ElName) ? string.Empty : ElName;
            }
            else
            {
                var addMessage = string.Empty;
                TreeNode newNode = string.IsNullOrEmpty(ElName) ? new TreeNode() : new TreeNode(ElName);
                if (_isElement)
                {
                    addMessage = ElementHelper.AddElement(_fullPath, Element,
                        RuntimeMongoDbContext.CurrentDocument,
                        RuntimeMongoDbContext.GetCurrentCollection());
                    newNode.Tag = Element;
                }
                else
                {
                    ElementHelper.AddArrayValue(_fullPath, ElValue,
                        RuntimeMongoDbContext.CurrentDocument,
                        RuntimeMongoDbContext.GetCurrentCollection());
                    newNode.Tag = ElValue;
                }
                if (!string.IsNullOrEmpty(addMessage))
                {
                    MyMessageBox.ShowMessage("Exception", addMessage);
                    return;
                }
                _selectNode.Nodes.Add(newNode);
            }
            Close();
        }
        /// <summary>
        ///     选择数据类型变化事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbDataType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!IsLoaded) return;
            ctlBsonValue1.DataTypeChanged((BsonValueEx.BasicType)cmbDataType.SelectedIndex);
        }
        /// <summary>
        ///     取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}