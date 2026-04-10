
using Uracle.Domain.Enums;

namespace Uracle.Infrastructure.Data.Extensions
{
    internal class InitialData
    {
        // ===== USER IDS =====
        public static readonly string AdminUserId = "3d5f8fc2-f243-44a5-ac68-f9ff5118d812";
        public static readonly string NormalUserId = "10886274-0c35-4600-9468-caf8827112cb";
        public static readonly string TestUser1Id = "a1b2c3d4-e5f6-7890-abcd-ef1234567890";
        public static readonly string TestUser2Id = "b2c3d4e5-f6g7-8901-bcde-f23456789012";

        // ===== GROUP IDS =====
        public static readonly string DefaultGroupId = "6fd0b8bf-7390-4617-b011-1c1ba2f9d384";
        public static readonly string TestGroupId = "a1b2c3d4-e5f6-7890-abcd-ef1234567891";

        public static IEnumerable<User> Users => new List<User>
        {
            new User
            {
                Id = AdminUserId,
                Username = "admin",
                Email = "admin@example.com",
                DisplayName = "Administrator",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = NormalUserId,
                Username = "user",
                Email = "user@example.com",
                DisplayName = "Regular User",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("User@123"),
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = TestUser1Id,
                Username = "testuser1",
                Email = "test1@example.com",
                DisplayName = "Test User 1",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                CreatedAt = DateTime.UtcNow
            },
            new User
            {
                Id = TestUser2Id,
                Username = "testuser2",
                Email = "test2@example.com",
                DisplayName = "Test User 2",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("Test@123"),
                CreatedAt = DateTime.UtcNow
            }
        };

        public static IEnumerable<StravaProfile> StravaProfiles => new List<StravaProfile>
        {
            new StravaProfile { Id = 1001, Username = "strava_admin", Firstname = "System", Lastname = "Admin" },
            new StravaProfile { Id = 1002, Username = "strava_user",  Firstname = "Default", Lastname = "User" },
            new StravaProfile { Id = 1003, Username = "strava_test1", Firstname = "Test", Lastname = "User1" },
            new StravaProfile { Id = 1004, Username = "strava_test2", Firstname = "Test", Lastname = "User2" }
        };

        public static IEnumerable<Group> Groups => new List<Group>
        {
            new Group
            {
                Id = DefaultGroupId,
                Name = "Default Admin Group",
                Description = "Default group with admin as group admin",
                IsPrivate = false,
                MemberCount = 4
            },
            new Group
            {
                Id = TestGroupId,
                Name = "Test Running Group",
                Description = "Test group for running contests",
                IsPrivate = false,
                MemberCount = 2
            }
        };

        public static IEnumerable<GroupMember> GroupMembers => new List<GroupMember>
        {
            new GroupMember
            {
                Id = Guid.NewGuid().ToString(),
                UserId = AdminUserId,
                GroupId = DefaultGroupId,
                Role = UserRole.admin,
                JoinedAt = DateTime.UtcNow
            },
            new GroupMember
            {
                Id = Guid.NewGuid().ToString(),
                UserId = NormalUserId,
                GroupId = DefaultGroupId,
                Role = UserRole.member,
                JoinedAt = DateTime.UtcNow
            },
            new GroupMember
            {
                Id = Guid.NewGuid().ToString(),
                UserId = TestUser1Id,
                GroupId = TestGroupId,
                Role = UserRole.admin,
                JoinedAt = DateTime.UtcNow
            },
            new GroupMember
            {
                Id = Guid.NewGuid().ToString(),
                UserId = TestUser2Id,
                GroupId = TestGroupId,
                Role = UserRole.member,
                JoinedAt = DateTime.UtcNow
            }
        };


        public static IEnumerable<IndividualContestActivity> CreateIndividualContestActivities(string individualContestId)
        {
            var activities = new List<IndividualContestActivity>
    {
        // Activities for NormalUserId - Sử dụng WorkoutActivity có sẵn
        new IndividualContestActivity
        {
            UserId = NormalUserId,
            ContestId = individualContestId,
            StravaUserId = 1002,
            Distance = 10.5,
            MovingTime = 3780,
            WorkoutType = "Run",
            Pace = 6.0,
            StartDate = DateTime.UtcNow.AddDays(-5),
            StravaActivityId = 1234567890123600,
            WorkoutActivityId = "workout4"  // DÙNG WORKOUTACTIVITY CÓ SẴN
        },
        new IndividualContestActivity
        {
            UserId = NormalUserId,
            ContestId = individualContestId,
            StravaUserId = 1002,
            Distance = 8.2,
            MovingTime = 2952,
            WorkoutType = "Run",
            Pace = 6.0,
            StartDate = DateTime.UtcNow.AddDays(-3),
            StravaActivityId = 1234567890123601,
            WorkoutActivityId = "workout5"  // DÙNG WORKOUTACTIVITY CÓ SẴN
        },
        new IndividualContestActivity
        {
            UserId = NormalUserId,
            ContestId = individualContestId,
            StravaUserId = 1002,
            Distance = 12.8,
            MovingTime = 4608,
            WorkoutType = "Run",
            Pace = 6.0,
            StartDate = DateTime.UtcNow.AddDays(-1),
            StravaActivityId = 1234567890123602,
            WorkoutActivityId = "workout6"  // DÙNG WORKOUTACTIVITY CÓ SẴN
        },
        
        // Activities for TestUser1Id - Sử dụng WorkoutActivity có sẵn
        new IndividualContestActivity
        {
            UserId = TestUser1Id,
            ContestId = individualContestId,
            StravaUserId = 1003,
            Distance = 9.8,
            MovingTime = 3528,
            WorkoutType = "Run",
            Pace = 6.0,
            StartDate = DateTime.UtcNow.AddDays(-4),
            StravaActivityId = 1234567890123603,
            WorkoutActivityId = "workout1"  // DÙNG WORKOUTACTIVITY CÓ SẴN
        },
        new IndividualContestActivity
        {
            UserId = TestUser1Id,
            ContestId = individualContestId,
            StravaUserId = 1003,
            Distance = 11.2,
            MovingTime = 4032,
            WorkoutType = "Run",
            Pace = 6.0,
            StartDate = DateTime.UtcNow.AddDays(-2),
            StravaActivityId = 1234567890123604,
            WorkoutActivityId = "workout2"  // DÙNG WORKOUTACTIVITY CÓ SẴN
        },
        new IndividualContestActivity
        {
            UserId = TestUser1Id,
            ContestId = individualContestId,
            StravaUserId = 1003,
            Distance = 7.5,
            MovingTime = 2700,
            WorkoutType = "Run",
            Pace = 6.0,
            StartDate = DateTime.UtcNow.AddDays(-1),
            StravaActivityId = 1234567890123605,
            WorkoutActivityId = "workout3"  // DÙNG WORKOUTACTIVITY CÓ SẴN
        }
    };

            // Set IDs để EF tracking
            for (int i = 0; i < activities.Count; i++)
            {
                activities[i].Id = $"individual-activity-{(i + 1):D3}";
            }

            return activities;
        }
        // ===== CONTESTS - KHÔNG CÓ ID CỐ ĐỊNH =====
        public static IEnumerable<Contest> Contests => new List<Contest>
        {
            Contest.Create(
                groupId: DefaultGroupId,
                createdBy: AdminUserId,
                name: "Individual Running Challenge",
                detail: "Individual running contest for testing",
                startAt: DateTime.UtcNow.AddDays(-7).AddYears(1),
                endAt: DateTime.UtcNow.AddDays(7).AddYears(1),
                contestType: ContestType.Individual,
                activityType: ActivityType.Run,
                minPace: 4.0,
                maxPace: 8.0,
                minDistance: 3.0
            ),
            Contest.Create(
                groupId: TestGroupId,
                createdBy: TestUser1Id,
                name: "Team Running Competition",
                detail: "Team-based running competition",
                startAt: DateTime.UtcNow.AddDays(-5),
                endAt: DateTime.UtcNow.AddDays(10),
                contestType: ContestType.Team,
                activityType: ActivityType.Run,
                minPace: 4.5,
                maxPace: 7.5,
                minDistance: 5.0
            )
        };

        // ===== WORKOUT ACTIVITIES =====
        public static IEnumerable<WorkoutActivity> WorkoutActivities => new List<WorkoutActivity>
        {
            new WorkoutActivity
            {
                Id = "workout1",
                UserId = TestUser1Id,
                StravaUserId = 1003,
                Distance = 7.2,
                MovingTime = 2400,
                WorkoutType = "Run",
                Pace = 5.6,
                StartDate = DateTime.UtcNow.AddDays(-2),
                StravaActivityId = 1234567890123456,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new WorkoutActivity
            {
                Id = "workout2",
                UserId = TestUser1Id,
                StravaUserId = 1003,
                Distance = 5.8,
                MovingTime = 1980,
                WorkoutType = "Run",
                Pace = 5.7,
                StartDate = DateTime.UtcNow.AddDays(-1),
                StravaActivityId = 1234567890123457,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new WorkoutActivity
            {
                Id = "workout3",
                UserId = TestUser2Id,
                StravaUserId = 1004,
                Distance = 6.5,
                MovingTime = 2100,
                WorkoutType = "Run",
                Pace = 5.4,
                StartDate = DateTime.UtcNow.AddDays(-1),
                StravaActivityId = 1234567890123458,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            },
            new WorkoutActivity
            {
                Id = "workout4",
                UserId = NormalUserId,
                StravaUserId = 1002,
                Distance = 8.1,
                MovingTime = 2700,
                WorkoutType = "Run",
                Pace = 5.6,
                StartDate = DateTime.UtcNow.AddDays(-3),
                StravaActivityId = 1234567890123459,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                UpdatedAt = DateTime.UtcNow.AddDays(-3)
            },
            new WorkoutActivity
            {
                Id = "workout5",
                UserId = NormalUserId,
                StravaUserId = 1002,
                Distance = 9.1,
                MovingTime = 2916,
                WorkoutType = "Run",
                Pace = 5.3,
                StartDate = DateTime.UtcNow.AddDays(-2),
                StravaActivityId = 1234567890123460,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new WorkoutActivity
            {
                Id = "workout6",
                UserId = AdminUserId,
                StravaUserId = 1001,
                Distance = 6.8,
                MovingTime = 2340,
                WorkoutType = "Run",
                Pace = 5.5,
                StartDate = DateTime.UtcNow.AddDays(-1),
                StravaActivityId = 1234567890123461,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                UpdatedAt = DateTime.UtcNow.AddDays(-1)
            }
        };

        // ===== FACTORY METHODS CHO ENTITIES CẦN ID THỰC TẾ =====

        public static IEnumerable<Team> CreateTeams(string teamContestId)
        {
            var teams = new List<Team>
            {
                Team.Create(
                    groupId: TestGroupId,
                    contestId: teamContestId,
                    name: "Alpha Runners"
                ),
                Team.Create(
                    groupId: TestGroupId,
                    contestId: teamContestId,
                    name: "Beta Sprinters"
                )
            };

            // Set IDs để EF tracking
            teams[0].Id = "alpha-team-id-001";
            teams[1].Id = "beta-team-id-002";

            return teams;
        }

        public static IEnumerable<ContestUser> CreateContestUsers(string individualContestId, string teamContestId) => new List<ContestUser>
        {
            // Individual contest participants
            new ContestUser
            {
                Id = Guid.NewGuid().ToString(),
                ContestId = individualContestId,
                UserId = AdminUserId,
                JoinedAt = DateTime.UtcNow.AddDays(-6)
            },
            new ContestUser
            {
                Id = Guid.NewGuid().ToString(),
                ContestId = individualContestId,
                UserId = NormalUserId,
                JoinedAt = DateTime.UtcNow.AddDays(-5)
            },
            // Team contest participants (all team members)
            new ContestUser
            {
                Id = Guid.NewGuid().ToString(),
                ContestId = teamContestId,
                UserId = TestUser1Id,
                JoinedAt = DateTime.UtcNow.AddDays(-4)
            },
            new ContestUser
            {
                Id = Guid.NewGuid().ToString(),
                ContestId = teamContestId,
                UserId = TestUser2Id,
                JoinedAt = DateTime.UtcNow.AddDays(-3)
            },
            new ContestUser
            {
                Id = Guid.NewGuid().ToString(),
                ContestId = teamContestId,
                UserId = NormalUserId,
                JoinedAt = DateTime.UtcNow.AddDays(-4)
            },
            new ContestUser
            {
                Id = Guid.NewGuid().ToString(),
                ContestId = teamContestId,
                UserId = AdminUserId,
                JoinedAt = DateTime.UtcNow.AddDays(-2)
            }
        };

        public static IEnumerable<TeamMember> CreateTeamMembers(string alphaTeamId, string betaTeamId)
        {
            var teamMembers = new List<TeamMember>
            {
                new TeamMember(
                    teamId: alphaTeamId,
                    userId: TestUser1Id,
                    role: UserRole.admin,
                    joinedAt: DateTime.UtcNow.AddDays(-4)
                ),
                new TeamMember(
                    teamId: alphaTeamId,
                    userId: TestUser2Id,
                    role: UserRole.member,
                    joinedAt: DateTime.UtcNow.AddDays(-3)
                ),
                new TeamMember(
                    teamId: betaTeamId,
                    userId: NormalUserId,
                    role: UserRole.admin,
                    joinedAt: DateTime.UtcNow.AddDays(-4)
                ),
                new TeamMember(
                    teamId: betaTeamId,
                    userId: AdminUserId,
                    role: UserRole.member,
                    joinedAt: DateTime.UtcNow.AddDays(-2)
                )
            };

            //// Set IDs để EF tracking
            //teamMembers[0].Id = "team-member-001";
            //teamMembers[1].Id = "team-member-002";
            //teamMembers[2].Id = "team-member-003";
            //teamMembers[3].Id = "team-member-004";

            return teamMembers;
        }

        public static IEnumerable<TeamMemberActivity> CreateTeamMemberActivities(string alphaTeamId, string betaTeamId, string teamContestId)
        {
            var activities = new List<TeamMemberActivity>
            {
                // Activities for TestUser1 in Alpha Team
                new TeamMemberActivity
                {
                    UserId = TestUser1Id,
                    TeamId = alphaTeamId,
                    ContestId = teamContestId,
                    StravaUserId = 1003,
                    Distance = 7.2,
                    MovingTime = 2400,
                    WorkoutType = "Run",
                    Pace = 5.6,
                    StartDate = DateTime.UtcNow.AddDays(-2),
                    StravaActivityId = 1234567890123456,
                    WorkoutActivityId = "workout1"
                },
                new TeamMemberActivity
                {
                    UserId = TestUser1Id,
                    TeamId = alphaTeamId,
                    ContestId = teamContestId,
                    StravaUserId = 1003,
                    Distance = 5.8,
                    MovingTime = 1980,
                    WorkoutType = "Run",
                    Pace = 5.7,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    StravaActivityId = 1234567890123457,
                    WorkoutActivityId = "workout2"
                },
                // Activities for TestUser2 in Alpha Team
                new TeamMemberActivity
                {
                    UserId = TestUser2Id,
                    TeamId = alphaTeamId,
                    ContestId = teamContestId,
                    StravaUserId = 1004,
                    Distance = 6.5,
                    MovingTime = 2100,
                    WorkoutType = "Run",
                    Pace = 5.4,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    StravaActivityId = 1234567890123458,
                    WorkoutActivityId = "workout3"
                },
                // Activities for NormalUserId in Beta Team
                new TeamMemberActivity
                {
                    UserId = NormalUserId,
                    TeamId = betaTeamId,
                    ContestId = teamContestId,
                    StravaUserId = 1002,
                    Distance = 8.1,
                    MovingTime = 2700,
                    WorkoutType = "Run",
                    Pace = 5.6,
                    StartDate = DateTime.UtcNow.AddDays(-3),
                    StravaActivityId = 1234567890123459,
                    WorkoutActivityId = "workout4"
                },
                new TeamMemberActivity
                {
                    UserId = NormalUserId,
                    TeamId = betaTeamId,
                    ContestId = teamContestId,
                    StravaUserId = 1002,
                    Distance = 9.1,
                    MovingTime = 2916,
                    WorkoutType = "Run",
                    Pace = 5.3,
                    StartDate = DateTime.UtcNow.AddDays(-2),
                    StravaActivityId = 1234567890123460,
                    WorkoutActivityId = "workout5"
                },
                // Activities for AdminUserId in Beta Team
                new TeamMemberActivity
                {
                    UserId = AdminUserId,
                    TeamId = betaTeamId,
                    ContestId = teamContestId,
                    StravaUserId = 1001,
                    Distance = 6.8,
                    MovingTime = 2340,
                    WorkoutType = "Run",
                    Pace = 5.5,
                    StartDate = DateTime.UtcNow.AddDays(-1),
                    StravaActivityId = 1234567890123461,
                    WorkoutActivityId = "workout6"
                }
            };

            // Set IDs để EF tracking
            for (int i = 0; i < activities.Count; i++)
            {
                activities[i].Id = $"activity-{(i + 1):D3}";
            }

            return activities;
        }
    }
}