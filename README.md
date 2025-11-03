# I. Database Design
erDiagram
  Users {
    ObjectId _id PK
    string username
    string password
    string email
    string displayName
    string bio
    number stravaId
    json stravaProfile
    string accessToken
    string refreshToken
    number expiresAt
    string jwtRefreshToken
    string resetToken
    datetime resetTokenExpiry
    bool mustChangePassword
    ObjectId[] groupIds
    datetime createdAt
    datetime updatedAt
  }

  Groups {
    ObjectId _id PK
    string name
    string description
    bool private
    string createdBy  // userId string
    number memberCount
    datetime createdAt
    datetime updatedAt
  }

  GroupMembers {
    ObjectId _id PK
    string groupId  // -> Groups._id
    string userId   // -> Users._id
    enum role       // admin|member
    datetime joinedAt
  }

  JoinRequests {
    ObjectId _id PK
    string groupId  // -> Groups._id
    string userId   // -> Users._id
    enum status     // pending|approved|rejected
    datetime requestedAt
    datetime processedAt
    string processedBy // userId
  }

  Contests {
    ObjectId _id PK
    string name
    ObjectId groupId   // -> Groups._id
    ObjectId createdBy // -> Users._id
    datetime startAt
    datetime endAt
    number numberOfParticipants
    number numberOfTeams
    ObjectId[] teamIds
    ObjectId[] participantIds
    string detail
    enum contestType   // Team|Individual
    enum activityType  // Run|Walk|Swim|Ride|All
    number minPace
    number maxPace
    number minDistance
    datetime createdAt
    datetime updatedAt
  }

  Teams {
    ObjectId _id PK
    ObjectId groupId   // -> Groups._id
    ObjectId contestId // -> Contests._id
    string name
    number numberOfMember
    number averagePace
    number totalDistance
    number totalTracklog
    number fastestPace
    number maxDistance
    datetime createdAt
    datetime updatedAt
  }

  WorkoutActivities {
    ObjectId _id PK
    ObjectId userId       // -> Users._id
    number stravaUserId
    number distance
    number movingTime
    string workoutType
    number pace
    datetime startDate
    datetime createdAt
    datetime updatedAt
    number stravaActivityId
  }

  TeamMemberActivities {
    ObjectId _id PK
    ObjectId userId            // -> Users._id
    ObjectId teamId            // -> Teams._id
    ObjectId contestId         // -> Contests._id
    number stravaUserId
    number distance
    number movingTime
    string workoutType
    number pace
    datetime startDate
    ObjectId workoutActivityId // -> WorkoutActivities._id
    number stravaActivityId
    datetime createdAt
    datetime updatedAt
  }

  IndividualContestActivities {
    ObjectId _id PK
    ObjectId userId            // -> Users._id
    ObjectId contestId         // -> Contests._id
    number stravaUserId
    number distance
    number movingTime
    string workoutType
    number pace
    datetime startDate
    ObjectId workoutActivityId // -> WorkoutActivities._id
    number stravaActivityId
    datetime createdAt
    datetime updatedAt
  }

  Events {
    ObjectId _id PK
    string aspect_type
    number event_time
    number object_id
    string object_type
    number owner_id
    number subscription_id
    json updates
    datetime received_at
    enum status        // Pending|Processing|Successful|Failed
    number attempts
    string error
  }

  %% Relationships
  Users ||--o{ GroupMembers : "member of"
  Groups ||--o{ GroupMembers : "has members"

  Users ||--o{ JoinRequests : "requests"
  Groups ||--o{ JoinRequests : "receives"

  Groups ||--o{ Contests : "owns"
  Users ||--o{ Contests : "creates"

  Contests ||--o{ Teams : "has"

  %% User membership in teams (array in Team.members)
  Users }o--o{ Teams : "members"

  Users ||--o{ WorkoutActivities : "performs"

  %% Activities mapped into contest/team context
  Users ||--o{ TeamMemberActivities : "has"
  Teams ||--o{ TeamMemberActivities : "aggregates"
  Contests ||--o{ TeamMemberActivities : "in contest"
  WorkoutActivities ||--o{ TeamMemberActivities : "derived from"

  Users ||--o{ IndividualContestActivities : "has"
  Contests ||--o{ IndividualContestActivities : "in contest"
  WorkoutActivities ||--o{ IndividualContestActivities : "derived from"

  %% Contest participants (participantIds)
  Users }o--o{ Contests : "participates"


  # II. Requirements

## Auth

- **`POST /api/register`** đăng ký.
- **`POST /api/login`** đăng nhập, set httpOnly cookies: **`accessToken`**, refreshToken.
- **`POST /api/refresh-token`** làm mới token qua cookie.
- **`POST /api/logout`** xóa cookies.
- **`POST /api/forgot-password`** yêu cầu reset password.
- **`PUT /api/change-password`** đổi mật khẩu (cần **`authenticateToken`**).
- **`GET /api/connect/strava`** chuyển hướng sang Strava OAuth (yêu cầu đã đăng nhập).
- **`GET /api/auth/strava/callback`** callback Strava, liên kết tài khoản.

## Groups

- CRUD nhóm: **`POST /api/groups`**, **`GET /api/groups/:id`**, **`PUT /api/groups/:id`**, **`DELETE /api/groups/:id`**.
- Danh sách nhóm có thống kê/preview contest: **`GET /api/groups`**.
- Contest theo group: **`GET /api/groups/:id/contests`**.
- Thành viên nhóm:
    - **`GET /api/groups/:id/members`**
    - **`POST /api/groups/:id/members`**
    - **`DELETE /api/groups/:id/members/:userId`**
    - **`PUT /api/groups/:id/members/:userId/role`**
    - Vai trò người dùng trong nhóm: **`GET /api/groups/:id/role`**
- Yêu cầu tham gia nhóm (join requests):
    - Tạo yêu cầu: **`POST /api/groups/:id/join`**
    - Danh sách pending: **`GET /api/groups/:id/requests`**
    - Duyệt: **`POST /api/groups/:id/requests/:userId/approve`**
    - Từ chối: **`POST /api/groups/:id/requests/:userId/reject`**
    - Trạng thái của user: **`GET /api/groups/:id/join-status`**
    - Thu hồi yêu cầu: **`DELETE /api/groups/:id/join`**

## Contest

- Contest:
    - **`GET /api/contests`** (theo nhóm của user).
    - **`POST /api/contests`**
    - **`GET /api/contests/:id`**
    - **`PUT /api/contests/:id`**
    - **`DELETE /api/contests/:id`**
    - Thành viên (individual contests):
        - **`GET /api/contests/:id/participants`**
        - Thêm 1: **`POST /api/contests/:contestId/participants`**
        - Thêm nhiều: **`POST /api/contests/:contestId/participants/bulk`**
        - Xóa: **`DELETE /api/contests/:contestId/participants/:participantId`**
        - Người có thể thêm: **`GET /api/contests/:contestId/available-participants`**
    - BXH cá nhân: **`GET /api/contests/:id/leaderboard`**
    - Hoạt động cá nhân trong contest: **`GET /api/contests/:id/users/:userId/activities`**

## Teams

- Theo contest: **`GET /api/contests/:id/teams`**
- Tạo team: **`POST /api/contests/:contestId/teams`**
- Thêm thành viên:
    - 1 người: **`POST /api/teams/:teamId/members`**
    - nhiều người: **`POST /api/teams/:teamId/members/bulk`**
- Xóa thành viên: **`DELETE /api/teams/:teamId/members/:userId`**
- Thống kê team trong contest:
    - **`GET /api/contests/:id/team-leaderboard?metric=totalDistance|averagePace|totalTracklog&limit=...`**
    - **`GET /api/contests/:contestId/teams/:teamId/stats`**
- Thống kê đội riêng: **`GET /api/teams/:teamId/stats`**

## **User**

- Hồ sơ: **`GET /api/user/profile`**, cập nhật: **`PUT /api/user/profile`**.
- Tìm kiếm: **`GET /api/users/search?query=...`**.
- Xem user: **`GET /api/users/:id`**.
- Hoạt động (đã đồng bộ): **`GET /api/user/activities`**.
- Đội của user: **`GET /api/user/teams`** hoặc **`GET /api/users/:userId/teams`**.

## **Webhook Strava**

• Verify: **`GET /api/webhook`** (Strava xác thực webhook).

• Nhận event: **`POST /api/webhook`**

## Admin/Dev

• Đồng bộ hoạt động thủ công theo batch: **`POST /api/admin/sync-activity-data`** (yêu cầu **`authenticateApiKey`**).


# III. Bounded Context
# **Bounded Contexts**

- **Identity & Access**
    - Entities: User
    - Responsibilities: AuthN/AuthZ, JWT refresh, password reset, Strava OAuth token storage/refresh.
    - External: Strava OAuth.
- **Groups & Membership**
    - Entities: Group, GroupMember, JoinRequest
    - Responsibilities: Group CRUD, privacy, membership, join-approve workflow, member counts.
- **Contests**
    - Entities: Contest
    - Responsibilities: Contest lifecycle, rules (activity type, pace/distance thresholds), enrollment (team or individual), timelines.
- **Teams**
    - Entities: Team (+ embedded TeamMember)
    - Responsibilities: Team creation within a contest, membership management, team metrics aggregation.
- **Activities Ingestion**
    - Entities: WorkoutActivity
    - Responsibilities: Ingest raw Strava workouts, normalize fields, deduplicate by **`stravaActivityId`**, compute pace, link to User.
- **Scoring & Leaderboards**
    - Entities: IndividualContestActivity, TeamMemberActivity
    - Responsibilities: Filter eligible workouts into contest activities, compute per-user and per-team stats, standings.
- **Strava Webhooks & Events**
    - Entities: Event
    - Responsibilities: Receive Strava events, idempotent processing, retries, DLQ/failed tracking.
- **Reporting & Analytics** (optional)
    - Derived views for dashboards, historical insights, trends.