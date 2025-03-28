
-- DROP TABLES IF EXISTS to reset schema (for dev/testing)
DROP TABLE IF EXISTS ComponentTags;
DROP TABLE IF EXISTS ComponentCategories;
DROP TABLE IF EXISTS SavedComponents;
DROP TABLE IF EXISTS Comments;
DROP TABLE IF EXISTS Versions;
DROP TABLE IF EXISTS Tags;
DROP TABLE IF EXISTS Categories;
DROP TABLE IF EXISTS UIComponents;
DROP TABLE IF EXISTS Users;

-- USERS table
CREATE TABLE Users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(50) NOT NULL UNIQUE,
    email VARCHAR(100) NOT NULL UNIQUE,
    password_hash TEXT NOT NULL,
    role VARCHAR(20) CHECK (role IN ('admin', 'creator', 'viewer')) DEFAULT 'viewer',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- UICOMPONENTS table
CREATE TABLE UIComponents (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description TEXT,
    code TEXT NOT NULL,
    preview_url TEXT,
    type VARCHAR(50),
    framework VARCHAR(50),
    created_by_user_id INTEGER REFERENCES Users(id) ON DELETE SET NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- CATEGORIES table
CREATE TABLE Categories (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) NOT NULL UNIQUE,
    description TEXT
);

-- COMPONENTCATEGORIES (many-to-many)
CREATE TABLE ComponentCategories (
    component_id INTEGER REFERENCES UIComponents(id) ON DELETE CASCADE,
    category_id INTEGER REFERENCES Categories(id) ON DELETE CASCADE,
    PRIMARY KEY (component_id, category_id)
);

-- TAGS table
CREATE TABLE Tags (
    id SERIAL PRIMARY KEY,
    name VARCHAR(50) NOT NULL UNIQUE
);

-- COMPONENTTAGS (many-to-many)
CREATE TABLE ComponentTags (
    component_id INTEGER REFERENCES UIComponents(id) ON DELETE CASCADE,
    tag_id INTEGER REFERENCES Tags(id) ON DELETE CASCADE,
    PRIMARY KEY (component_id, tag_id)
);

-- SAVED COMPONENTS
CREATE TABLE SavedComponents (
    user_id INTEGER REFERENCES Users(id) ON DELETE CASCADE,
    component_id INTEGER REFERENCES UIComponents(id) ON DELETE CASCADE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (user_id, component_id)
);

-- COMMENTS
CREATE TABLE Comments (
    id SERIAL PRIMARY KEY,
    component_id INTEGER REFERENCES UIComponents(id) ON DELETE CASCADE,
    user_id INTEGER REFERENCES Users(id) ON DELETE SET NULL,
    content TEXT NOT NULL,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- VERSIONS
CREATE TABLE Versions (
    id SERIAL PRIMARY KEY,
    component_id INTEGER REFERENCES UIComponents(id) ON DELETE CASCADE,
    code TEXT NOT NULL,
    change_log TEXT,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);


-- Insert sample users
INSERT INTO Users (username, email, password_hash, role) VALUES
('chiengio', 'chiengio@example.com', 'hashed_pw_1', 'admin'),
('frontenddev', 'frontend@example.com', 'hashed_pw_2', 'creator');

-- Insert categories
INSERT INTO Categories (name, description) VALUES
('Buttons', 'All types of buttons'),
('Cards', 'Card layouts and content blocks'),
('Navigation', 'Navbar, sidebar, breadcrumbs, etc.');

-- Insert tags
INSERT INTO Tags (name) VALUES
('responsive'),
('tailwind'),
('dark-mode');

-- Insert UI components
INSERT INTO UIComponents (name, description, code, preview_url, type, framework, created_by_user_id) VALUES
('Primary Button', 'A standard button with primary style', '<button class="btn-primary">Click me</button>', 'https://example.com/button.png', 'component', 'HTML/CSS', 1),
('Responsive Navbar', 'Navbar with responsive toggle', '<nav>...</nav>', 'https://example.com/navbar.png', 'component', 'Tailwind', 2);

-- ComponentCategories
INSERT INTO ComponentCategories (component_id, category_id) VALUES
(1, 1), -- Primary Button → Buttons
(2, 3); -- Responsive Navbar → Navigation

-- ComponentTags
INSERT INTO ComponentTags (component_id, tag_id) VALUES
(1, 1), -- responsive
(2, 1), -- responsive
(2, 2); -- tailwind

-- Saved Components
INSERT INTO SavedComponents (user_id, component_id) VALUES
(2, 1);

-- Comments
INSERT INTO Comments (component_id, user_id, content) VALUES
(1, 2, 'Great reusable button!');

-- Versions
INSERT INTO Versions (component_id, code, change_log) VALUES
(1, '<button class="btn-primary">Click me</button>', 'Initial version'),
(1, '<button class="btn-primary hover:bg-blue-600">Click me</button>', 'Added hover effect');
