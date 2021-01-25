PRAGMA foreign_keys = ON;

CREATE TABLE TodoLists (
    Id PRIMARY KEY,
    Name TEXT NOT NULL
);

CREATE TABLE TodoItems (
    Id PRIMARY KEY,
    Value TEXT NOT NULL,
    TodoListId TEXT NOT NULL,
    FOREIGN KEY (TodoListId)
        REFERENCES TodoLists (Id)
);