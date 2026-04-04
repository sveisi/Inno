using Inno.Types;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inno.ViewModels
{
    public class UserListView
    {
        public string UserName { get; set; }

        //همیشه نام کاربر و مشتری سینک میشود
        [DisplayName("Name")]
        public string FullName { get; set; }

        [DisplayName("Active")]
        public bool IsActive { get; set; }
    }
}