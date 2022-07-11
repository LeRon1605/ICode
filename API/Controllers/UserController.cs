using API.Models.DTO;
using API.Models.Entity;
using API.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _userRepository;
        public UserController(IUnitOfWork unitOfWork,IUserRepository userRepository)
        {
            _unitOfWork = unitOfWork;
            _userRepository = userRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult GetAll()
        {
            return Ok(new
            {
                status = true,
                data = _userRepository.FindAll()
            }); 
        }   

        [HttpGet("{ID}")]
        public IActionResult GetByID(string ID)
        {
            if (string.IsNullOrEmpty(ID))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid ID"
                });
            }
            User user = _userRepository.FindSingle(user => user.ID == ID);
            if (user == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Không tìm thấy User"
                });
            }
            else
            {
                return Ok(new
                {
                    status = true,
                    message = "",
                    data = new UserDTO
                    {
                        ID = user.ID,
                        Username = user.Username,
                        Email = user.Email,
                        CreatedAt = user.CreatedAt,
                        UpdatedAt = user.UpdatedAt
                    }
                });
            }
        }

        [HttpPut("{ID}")]
        public IActionResult Update(string ID, UserUpdate input)
        {
            if (string.IsNullOrEmpty(ID))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid ID"
                });
            }
            User user = _userRepository.FindSingle(user => user.ID == ID);
            if (user == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Không tồn tại user"
                });
            }    
            if (!string.IsNullOrEmpty(input.Username) && _userRepository.isExist(user => user.Username == input.Username))
            {
                return Conflict(new
                {
                    status = false,
                    message = "Username đã tồn tại không thể cập nhật"
                });
            }
            try
            {
                user.Username = (string.IsNullOrEmpty(input.Username)) ? user.Username : input.Username;
                _unitOfWork.Commit();
                return NoContent();
            }
            catch(Exception e)
            {
                return BadRequest(new
                {
                    status = false,
                    message = e.Message
                });
            }
        }

        [HttpDelete("{ID}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(string ID)
        {
            if (string.IsNullOrEmpty(ID))
            {
                return BadRequest(new
                {
                    status = false,
                    message = "Invalid ID"
                });
            }    
            User user = _userRepository.FindSingle(user => user.ID == ID);
            if (user == null)
            {
                return NotFound(new
                {
                    status = false,
                    message = "Không tồn tại user"
                });
            }
            else
            {
                try
                {
                    _userRepository.Remove(user);
                    _unitOfWork.Commit();
                    return NoContent();
                }
                catch (Exception e)
                {
                    return BadRequest(new
                    {
                        status = false,
                        message = e.Message
                    });
                }
            }
        }
    }
}
