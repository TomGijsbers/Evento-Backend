﻿namespace WebApplication3.Models;

public class EventRegistration
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public User User { get; set; }

    public int EventId { get; set; }
    public Event Event { get; set; }

    public DateTime RegisteredAt { get; set; }
}