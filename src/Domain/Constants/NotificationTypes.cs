﻿namespace Domain.Constants;

public enum NotificationTypes : byte
{
    Undifined = 0,
    QuestionAnswered = 1,
    SimilarCreated = 2,
    QuizCreated = 3,
    Everbody = 4,
    HomeWork = 5,
    QuestionOcr = 6,
    BookPreparing = 7,
    BookReady = 8,
}