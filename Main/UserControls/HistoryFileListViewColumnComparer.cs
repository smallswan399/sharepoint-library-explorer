using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Main.Services.Domains;

namespace Main.UserControls
{
    class HistoryFileListViewColumnComparer : IComparer, IComparer<ListViewItem>
    {
        private HistoryFileListViewColumns column;
        private SortOrder sortOrder;

        public int Compare(object x, object y)
        {
            return Compare(x as ListViewItem, y as ListViewItem);
        }

        public int Compare(ListViewItem x, ListViewItem y)
        {
            long minus = 0;
            var result = 0;
            var version1 = x.Tag as SharePointFileVersion;
            var version2 = y.Tag as SharePointFileVersion;

            switch (column)
            {
                case HistoryFileListViewColumns.Version:
                    var minus1 = double.Parse(version1.VersionLabel) - double.Parse(version2.VersionLabel);
                    if (minus1 == 0)
                    {
                        minus = 0;
                    }
                    if (minus1 > 0)
                    {
                        minus = 1;
                    }
                    if (minus1 < 0)
                    {
                        minus = -1;
                    }
                    break;
                case HistoryFileListViewColumns.CreatedDate:
                    minus = DateTime.Compare(version1.Created, version2.Created);
                    break;
                case HistoryFileListViewColumns.Author:
                    minus = String.CompareOrdinal(version1.CreatedBy, version2.CreatedBy);
                    break;
                case HistoryFileListViewColumns.Size:
                    if (version1.Size != null && version2.Size != null)
                    {
                        if (version1.Size == version2.Size)
                        {
                            minus = 0;
                        }
                        if (version1.Size > version2.Size)
                        {
                            minus = 1;
                        }
                        if (version1.Size < version2.Size)
                        {
                            minus = -1;
                        }
                    }
                    if (version1.Size != null && version2.Size == null)
                    {
                        minus = 1;
                    }
                    if (version2.Size != null && version1.Size == null)
                    {
                        minus = -1;
                    }
                    break;
                case HistoryFileListViewColumns.Comment:
                    minus = String.CompareOrdinal(version1.CheckInComment, version2.CheckInComment);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (minus == 0)
            {
                result = 0;
            }
            else
            {
                if (minus > 0)
                {
                    result = 1;
                }
                else
                {
                    if (minus < 0)
                    {
                        result = -1;
                    }
                }

            }
            if (sortOrder == SortOrder.Descending)
                result = 0 - result;

            return result;
        }

        public HistoryFileListViewColumnComparer(HistoryFileListViewColumns col, SortOrder order)
        {
            column = col;
            sortOrder = order;
        }
    }
}