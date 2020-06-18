using tvz2api_cqrs.Common;
using tvz2api_cqrs.Implementation.Commands;
using tvz2api_cqrs.Implementation.Queries;
using tvz2api_cqrs.QueryModels;
using tvz2api_cqrs.Models;
using tvz2api_cqrs.Infrastructure.CommandHandlers;
using tvz2api_cqrs.Infrastructure.Commands;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using tvz2api_cqrs.Custom;
using tvz2api_cqrs.Enumerations;
using Newtonsoft.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace tvz2api_cqrs.Implementation.CommandHandlers
{
  public class CourseCommandHandler :
    ICommandHandlerAsync<CourseCreateNewSidebarCommand>,
    ICommandHandlerAsync<CourseDeleteSidebarCommand>,
    ICommandHandlerAsync<CourseCreateDiscussionCommand>,
    ICommandHandlerAsync<CourseDiscussionReplyCommand>,
    ICommandHandlerAsync<CourseDeleteDiscussionCommand>,
    ICommandHandlerAsync<CourseUpdateLandingPageCommand>,
    ICommandHandlerAsync<CourseUpdatePasswordCommand>,
    ICommandHandlerAsync<CourseCreateCommand, int>
  {
    private readonly lmsContext _context;
    private readonly IConfiguration _configuration;

    public CourseCommandHandler(lmsContext context, IConfiguration configuration)
    {
      _context = context;
      _configuration = configuration;
    }

    public async Task HandleAsync(CourseCreateNewSidebarCommand command)
    {
      _context.SidebarContent.Add(new SidebarContent()
      {
        CourseId = command.CourseId,
        Title = command.Title
      });
      await _context.SaveChangesAsync();
    }

    public async Task HandleAsync(CourseUpdatePasswordCommand command)
    {
      var course = _context.Course.FirstOrDefault(x => x.Id == command.CourseId);
      course.Password = command.NewPassword;
      await _context.SaveChangesAsync();
    }

    public async Task HandleAsync(CourseDiscussionReplyCommand command)
    {
      _context.DiscussionComment.Add(new DiscussionComment()
      {
        Content = command.Content,
        DiscussionId = command.DiscussionId,
        SubmittedAt = DateTime.Now,
        SubmittedById = command.SubmittedById
      });
      await _context.SaveChangesAsync();
    }

    public async Task<ICommandResult<int>> HandleAsync(CourseCreateCommand command)
    {
      if (_context.Course.Any(x => String.Equals(x.Name.ToLower(), command.Name.ToLower())))
      {
        throw new Exception("This name is already used!");
      }

      var newCourse = new Course()
      {
        Name = command.Name,
        MadeById = command.CreatedById,
        Password = command.Password,
        SpecializationId = command.SpecializationId
      };

      _context.Course.Add(newCourse);
      await _context.SaveChangesAsync();

      var privileges = new List<UserCoursePrivilege>() {
        new UserCoursePrivilege()
        {
          CourseId = newCourse.Id,
          PrivilegeId = (int)PrivilegeEnum.CanManageCourse,
          UserId = command.CreatedById
        },
        new UserCoursePrivilege()
        {
          CourseId = newCourse.Id,
          PrivilegeId = (int)PrivilegeEnum.IsInvolvedWithCourse,
          UserId = command.CreatedById
        }
      };

      _context.Subscription.Add(new Subscription()
      {
        CourseId = newCourse.Id,
        JoinedAt = DateTime.Now,
        UserId = command.CreatedById,
        Blacklisted = false
      });

      _context.UserCoursePrivilege.AddRange(privileges);
      await _context.SaveChangesAsync();

      return CommandResult<int>.Success(newCourse.Id);
    }

    public async Task HandleAsync(CourseUpdateLandingPageCommand command)
    {
      var course = _context.Course.FirstOrDefault(x => x.Id == command.CourseId);
      course.LandingPage = command.Content;
      await _context.SaveChangesAsync();
    }

    public async Task HandleAsync(CourseDeleteDiscussionCommand command)
    {
      var discussion = await _context
        .Discussion
        .Include(t => t.DiscussionComment)
        .FirstOrDefaultAsync(x => x.Id == command.DiscussionId);

      _context.DiscussionComment.RemoveRange(discussion.DiscussionComment);
      _context.Discussion.Remove(discussion);

      await _context.SaveChangesAsync();
    }

    public async Task HandleAsync(CourseCreateDiscussionCommand command)
    {
      var discussion = new Discussion()
      {
        CourseId = command.CourseId,
        Content = command.Body,
        SubmittedAt = DateTime.Now,
        SubmittedById = command.SubmittedById
      };

      _context.Discussion.Add(discussion);
      await _context.SaveChangesAsync();
    }

    public async Task HandleAsync(CourseDeleteSidebarCommand command)
    {
      var sidebar = await _context
        .SidebarContent
        .Include(t => t.SidebarContentFile)
        .FirstOrDefaultAsync(x => x.Id == command.SidebarId);

      _context.SidebarContentFile.RemoveRange(sidebar.SidebarContentFile);
      _context.Remove(sidebar);

      await _context.SaveChangesAsync();
    }
  }
}
