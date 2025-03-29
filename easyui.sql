
-- DROP ALL TABLES FOR CLEAN MIGRATION
DROP TABLE IF EXISTS ComponentTags;
DROP TABLE IF EXISTS ComponentCategories;
DROP TABLE IF EXISTS SavedComponents;
DROP TABLE IF EXISTS Comments;
DROP TABLE IF EXISTS Versions;
DROP TABLE IF EXISTS Tags;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS UIComponents;

DROP TABLE IF EXISTS AspNetRoleClaims;
DROP TABLE IF EXISTS AspNetUserClaims;
DROP TABLE IF EXISTS AspNetUserLogins;
DROP TABLE IF EXISTS AspNetUserTokens;
DROP TABLE IF EXISTS AspNetUserRoles;
DROP TABLE IF EXISTS AspNetRoles;
DROP TABLE IF EXISTS AspNetUsers;

-- ASP.NET Identity Tables

CREATE TABLE AspNetUsers (
    Id UUID PRIMARY KEY,
    UserName VARCHAR(256),
    NormalizedUserName VARCHAR(256),
    Email VARCHAR(256),
    NormalizedEmail VARCHAR(256),
    EmailConfirmed BOOLEAN NOT NULL DEFAULT FALSE,
    PasswordHash TEXT,
    SecurityStamp TEXT,
    ConcurrencyStamp TEXT,
    PhoneNumber TEXT,
    PhoneNumberConfirmed BOOLEAN NOT NULL DEFAULT FALSE,
    TwoFactorEnabled BOOLEAN NOT NULL DEFAULT FALSE,
    LockoutEnd TIMESTAMP,
    LockoutEnabled BOOLEAN NOT NULL DEFAULT TRUE,
    AccessFailedCount INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE AspNetRoles (
    Id UUID PRIMARY KEY,
    Name VARCHAR(256),
    NormalizedName VARCHAR(256),
    ConcurrencyStamp TEXT
);

CREATE TABLE AspNetUserRoles (
    UserId UUID NOT NULL,
    RoleId UUID NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserClaims (
    Id SERIAL PRIMARY KEY,
    UserId UUID NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserLogins (
    LoginProvider VARCHAR(128) NOT NULL,
    ProviderKey VARCHAR(128) NOT NULL,
    ProviderDisplayName TEXT,
    UserId UUID NOT NULL,
    PRIMARY KEY (LoginProvider, ProviderKey),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetUserTokens (
    UserId UUID NOT NULL,
    LoginProvider VARCHAR(128) NOT NULL,
    Name VARCHAR(128) NOT NULL,
    Value TEXT,
    PRIMARY KEY (UserId, LoginProvider, Name),
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id) ON DELETE CASCADE
);

CREATE TABLE AspNetRoleClaims (
    Id SERIAL PRIMARY KEY,
    RoleId UUID NOT NULL,
    ClaimType TEXT,
    ClaimValue TEXT,
    FOREIGN KEY (RoleId) REFERENCES AspNetRoles(Id) ON DELETE CASCADE
);

-- UI Component System

CREATE TABLE UIComponents (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    code TEXT NOT NULL,
    preview_url TEXT,
    type VARCHAR(50),
    framework VARCHAR(50),
    created_by_user_id UUID REFERENCES AspNetUsers(Id) ON DELETE SET NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT
);

CREATE TABLE ComponentCategories (
    component_id INTEGER REFERENCES UIComponents(id) ON DELETE CASCADE,
    category_id INTEGER REFERENCES Categories(id) ON DELETE CASCADE,
    PRIMARY KEY (component_id, category_id)
);

CREATE TABLE Tags (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE ComponentTags (
    component_id INTEGER REFERENCES UIComponents(id) ON DELETE CASCADE,
    tag_id INTEGER REFERENCES Tags(id) ON DELETE CASCADE,
    PRIMARY KEY (component_id, tag_id)
);

CREATE TABLE SavedComponents (
    user_id UUID REFERENCES AspNetUsers(Id) ON DELETE CASCADE,
    component_id INTEGER REFERENCES UIComponents(id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, component_id)
);

CREATE TABLE Comments (
    id SERIAL PRIMARY KEY,
    component_id INTEGER REFERENCES UIComponents(id) ON DELETE CASCADE,
    user_id UUID REFERENCES AspNetUsers(Id) ON DELETE SET NULL,
    content TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE Versions (
    id SERIAL PRIMARY KEY,
    component_id INTEGER REFERENCES UIComponents(id) ON DELETE CASCADE,
    code TEXT NOT NULL,
    change_log TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


-- SAMPLE DATA

-- Sample Roles
INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) VALUES
('08778050-cc3e-4c53-bf68-9023def286d8', 'Admin', 'ADMIN', '7fb4e2bb-b93c-41d4-848e-1ef61fb5eb17'),
('7e406915-73fe-448b-a90c-5b45fe5c497b', 'Creator', 'CREATOR', '8a662ac8-1375-4169-b27a-7423d793dc34');

-- Sample Users
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp)
VALUES
('7fd5f7a4-4874-465a-9e00-da0571e20640', 'chiengio', 'CHIENGIO', 'chiengio@example.com', 'CHIENGIO@EXAMPLE.COM', TRUE, 'hashed_pw_1', '33b98621-35da-4944-8b0d-81bac38b67c2', 'd37ae0b4-1615-412e-8d0e-6aa73a6665ee'),
('e3768a4a-9ef4-4ccf-ae83-b6e9f8301679', 'frontenddev', 'FRONTENDDEV', 'frontend@example.com', 'FRONTEND@EXAMPLE.COM', TRUE, 'hashed_pw_2', '8c828c6b-823c-48ca-b1d0-e9fcea9ad471', '5c191536-8a12-4b5b-95b7-4c1db4a59b82');

-- Assign Roles to Users
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES
('7fd5f7a4-4874-465a-9e00-da0571e20640', '08778050-cc3e-4c53-bf68-9023def286d8'),
('e3768a4a-9ef4-4ccf-ae83-b6e9f8301679', '7e406915-73fe-448b-a90c-5b45fe5c497b');

-- Insert Categories
INSERT INTO Categories (name, description) VALUES
('Buttons', 'All types of buttons'),
('Cards', 'Card layouts and content blocks'),
('Navigation', 'Navbar, sidebar, breadcrumbs, etc.');

-- Insert Tags
INSERT INTO Tags (name) VALUES
('responsive'),
('tailwind'),
('dark-mode');

-- Insert UI Components
INSERT INTO UIComponents (name, description, code, preview_url, type, framework, created_by_user_id) VALUES
('Primary Button', 'A standard button with primary style', '<button class="btn-primary">Click me</button>', 'https://example.com/button.png', 'component', 'HTML/CSS', '7fd5f7a4-4874-465a-9e00-da0571e20640'),
('Responsive Navbar', 'Navbar with responsive toggle', '<nav class="navbar">...</nav>', 'https://example.com/navbar.png', 'component', 'Tailwind', 'e3768a4a-9ef4-4ccf-ae83-b6e9f8301679');

-- Link Components to Categories
INSERT INTO ComponentCategories (component_id, category_id) VALUES
(1, 1), -- Primary Button -> Buttons
(2, 3); -- Navbar -> Navigation

-- Link Tags to Components
INSERT INTO ComponentTags (component_id, tag_id) VALUES
(1, 1), -- responsive
(2, 1), -- responsive
(2, 2); -- tailwind

-- Save Favorite Component
INSERT INTO SavedComponents (user_id, component_id) VALUES
('e3768a4a-9ef4-4ccf-ae83-b6e9f8301679', 1);

-- Comments
INSERT INTO Comments (component_id, user_id, content) VALUES
(1, 'e3768a4a-9ef4-4ccf-ae83-b6e9f8301679', 'Great reusable button!');

-- Versions
INSERT INTO Versions (component_id, code, change_log) VALUES
(1, '<button class="btn-primary">Click me</button>', 'Initial version'),
(1, '<button class="btn-primary hover:bg-blue-600">Click me</button>', 'Added hover effect');
