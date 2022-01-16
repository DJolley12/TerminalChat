namespace TerminalChatLib
{
    public interface IEncryptionService
    {
        public string Decrypt(string phrase);
        public string Encrypt(string phrase);
    }
}
