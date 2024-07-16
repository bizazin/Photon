namespace Utils
{
    public class Enumerators
    {
        
        public enum TeamRole
        {
            MyPlayer,
            AllyPlayer,
            EnemyPlayer,
        }

        public enum PlayerProperty
        {
            LocalPlayersSpawned,
            PlayerHP,
            IsDead,
            PlayerImmutableData,
            LocalPoolsPrepared
        }
        
        public enum Character
        {
            Primo,
            Voron,
            Rocket
        }

        public enum GameResult
        {
            Win = 0,
            Lose = 1,
            Draw = 2
        }    
        
        public enum LogLevel
        {
            Debug,
            Info,
            Warning,
            Error,
            Critical
        }
        
        public enum EGameDataType
        {
            UserData = 0
        }
        
        public enum ELanguage
        {
            Unknown = 0,
            English = 1,
            Bulgarian = 2,
            Hungarian = 3,
            Greek = 4,
            Danish = 5,
            Indonesian = 6,
            Spanish = 7,
            Italian = 8,
            Chinese = 9,
            Korean = 10,
            German = 11,
            Dutch = 12,
            Polish = 13,
            Portuguese = 14,
            Romanian = 15,
            Russian = 16,
            Turkish = 17,
            Ukrainian = 18,
            Finnish = 19,
            French = 20,
            Czech = 21,
            Swedish = 22,
            Estonian = 23,
            Japanese = 24
        }
        
        public enum ESpreadsheetDataType
        {
            Localization = 0,
            First = 1,
            Second = 2
        }
        
        public enum EWindow
        {
            MainMenu = 0,
            Loading = 2,
            MatchInfo = 3,
            PreviewMatchAnimation = 4,
            MatchResults = 5,
            SelectCharacter
        }
        
        public enum ESoundtrackName
        {
        }
        
        public enum EVolumeType
        {
            Master = 0,
            Video = 1,
            Effects = 2,
            Music = 3
        }
        
        public enum ESoundFxName
        {
            NewMessage = 0,
            CriticalClick = 1,
            PlanetOpened = 2,
            NewContentAngle = 3,
            UpgradeBought = 4,  
            PointerEnter = 5,
            CommonClick = 6,
            UiClick = 7,
            ContentWheelRotate = 8,
            MiniGameAimClick1 = 9,
            MiniGameAimClick2 = 10,
            MiniGameAimClick3 = 11,
            MiniGameAimClick4 = 12,
            MiniGameAimClick5 = 13,
            AlanaWins1 = 14,
            AlanaWins2 = 15,
            DonationRecieved = 16,
            NewPoseAngleUnlocked = 17,
            ArticleTyping = 18
        }
    }
}