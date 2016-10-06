using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using Main.Services.Domains;

namespace Main.UserControls
{
    internal class FileListViewColumnComparer : IComparer, IComparer<FilesListViewItem>
    {
        public FileListViewColumnComparer(FileListViewColumns col, SortOrder order)
        {
            column = col;
            sortOrder = order;
        }

        public int Compare(object x, object y)
        {
            return Compare((FilesListViewItem)x, (FilesListViewItem)y);
        }

        public int Compare(FilesListViewItem x, FilesListViewItem y)
        {
            long minus = 0;
            var result = 0;
            var file1 = x.SharePointLibraryItem;
            var file2 = y.SharePointLibraryItem;

            if (file1 is SharePointFile && file2 is SharePointFile)
            {
                switch (column)
                {
                    case FileListViewColumns.Name:
                        minus = String.CompareOrdinal(file1.Name, file2.Name);
                        break;
                    case FileListViewColumns.Created:
                        minus = DateTime.Compare(file1.CreatedDateTime, file2.CreatedDateTime);
                        break;
                    case FileListViewColumns.Author:
                        minus = String.CompareOrdinal(file1.Author, file2.Author);
                        break;
                    case FileListViewColumns.Size:
                        minus = (file1 as SharePointFile).FileSize - (file2 as SharePointFile).FileSize;
                        break;
                    case FileListViewColumns.CheckOutTo:
                        minus = String.CompareOrdinal((file1 as SharePointFile).CheckoutUser, (file2 as SharePointFile).CheckoutUser);
                        break;
                    case FileListViewColumns.LastModified:
                        minus = DateTime.Compare(file1.LastModifiedDateTime, file2.LastModifiedDateTime);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else
            {
                if (file1 is SharePointFile && file2 is SharePointFolder)
                {
                    minus = 1;
                }
                if (file2 is SharePointFile && file1 is SharePointFolder)
                {
                    minus = -1;
                }
                if (file2 is SharePointFolder && file1 is SharePointFolder)
                {
                    minus = String.CompareOrdinal(file1.Name, file2.Name);
                }
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

        private FileListViewColumns column;
        private SortOrder sortOrder;
    }
}
