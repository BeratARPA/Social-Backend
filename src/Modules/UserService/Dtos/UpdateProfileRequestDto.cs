namespace UserService.Dtos
{
    public class UpdateProfileRequestDto
    {      
        public string Username { get; set; }   
        public string FullName { get; set; }
        public string DisplayName { get; set; }  
        public string Bio { get; set; }
        public string Website { get; set; }       
        public string Location { get; set; }
        public bool IsPrivate { get; set; }
    }
}
