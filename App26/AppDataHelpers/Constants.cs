namespace App26
{
    internal class Constants
    {
        public const string USERS_TABLE_ID = "Users";
        public const string CONVERSATION_TABLE_ID = "Conversations";

        //USER ATTRIBUTES
        public const string USER_ID = "User_Id";
        public const string USER_NAME = "User_Name";
        public const string USER_EMAIL = "User_Email";
        public const string USER_PASSWORD = "User_Password";
        public const string USER_PHOTO = "User_Photo";
        public const string USER_REF = "User_Ref";

        //LOCAL ATRIBUTES
        public const string IS_LOGGED = "User_Logged";

        //CONVERSATION ATRIBUTES
        public const string PARTICIPANTS = "Participants_Id";
        public const string CONVERSATION_ID = "Conversation_Id";
        public const string SENDER_ID = "Sender_Id";
        public const string MESSAGES = "Messages";
        public const string LAST_MESSAGE = "Last_Message";
        public const string IS_GROUP = "Is_Group";
        public const string GROUP_NAME = "Group_Name";
        public const string GROUP_PHOTO = "Group_Photo";

        //MESSAGES ATRIBUTES
        public const string MESSAGE_TEXT = "Message_Text";
        public const string MESSAGE_DATE = "Message_Date";
    }
}