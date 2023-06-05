SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


CREATE TABLE `courses` (
  `course_uid` varchar(48) NOT NULL DEFAULT uuid(),
  `title` varchar(255) NOT NULL,
  `description` text NOT NULL,
  `overview` longtext DEFAULT NULL,
  `imgsrc` varchar(255) NOT NULL DEFAULT '/Assets/Courses/lorem.png',
  `instructor_uid` varchar(48) NOT NULL,
  `created_at` timestamp NOT NULL DEFAULT current_timestamp(),
  `featured` tinyint(1) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

CREATE TABLE `lessons` (
  `lesson_uid` varchar(48) NOT NULL DEFAULT uuid(),
  `course_uid` varchar(48) NOT NULL,
  `lesson_name` varchar(255) NOT NULL,
  `lesson_content` longtext NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


CREATE TABLE `orders` (
  `order_uid` varchar(48) NOT NULL DEFAULT uuid(),
  `user_uid` varchar(48) NOT NULL,
  `order_date` date NOT NULL,
  `total_amount` decimal(10,2) NOT NULL,
  `status` enum('Completed','Pending','Cancelled') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


CREATE TABLE `users` (
  `user_uid` varchar(48) NOT NULL DEFAULT uuid(),
  `username` varchar(255) NOT NULL,
  `password` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `full_name` varchar(255) NOT NULL,
  `role` enum('student','instructor','admin') NOT NULL DEFAULT 'student',
  `imgsrc` varchar(255) NOT NULL DEFAULT '/Assets/Users/img.png',
  `bio` longtext NOT NULL DEFAULT 'User',
  `created_at` timestamp NOT NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

INSERT INTO `users` (`username`, `password`, `email`, `full_name`, `role`, `bio`) VALUES
('admin', '8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', 'admin@example.com', 'John Doe', 'admin', 'Administrator of the site');



CREATE TABLE `user_courses` (
  `user_uid` varchar(48) NOT NULL,
  `course_uid` varchar(48) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;


ALTER TABLE `courses`
  ADD PRIMARY KEY (`course_uid`),
  ADD KEY `instructor_id` (`instructor_uid`);

ALTER TABLE `lessons`
  ADD PRIMARY KEY (`lesson_uid`),
  ADD KEY `course_id` (`course_uid`);


ALTER TABLE `orders`
  ADD PRIMARY KEY (`order_uid`),
  ADD KEY `user_uid` (`user_uid`);

ALTER TABLE `users`
  ADD PRIMARY KEY (`user_uid`);

ALTER TABLE `user_courses`
  ADD PRIMARY KEY (`user_uid`,`course_uid`),
  ADD KEY `course_id` (`course_uid`);

ALTER TABLE `courses`
  ADD CONSTRAINT `courses_ibfk_1` FOREIGN KEY (`instructor_uid`) REFERENCES `users` (`user_uid`);


ALTER TABLE `lessons`
  ADD CONSTRAINT `lessons_ibfk_1` FOREIGN KEY (`course_uid`) REFERENCES `courses` (`course_uid`);


ALTER TABLE `orders`
  ADD CONSTRAINT `orders_ibfk_1` FOREIGN KEY (`user_uid`) REFERENCES `users` (`user_uid`);


ALTER TABLE `user_courses`
  ADD CONSTRAINT `user_courses_ibfk_1` FOREIGN KEY (`user_uid`) REFERENCES `users` (`user_uid`),
  ADD CONSTRAINT `user_courses_ibfk_2` FOREIGN KEY (`course_uid`) REFERENCES `courses` (`course_uid`);
COMMIT;
