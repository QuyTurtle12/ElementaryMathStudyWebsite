using ElementaryMathStudyWebsite.Contract.UseCases.DTOs.SubjectDtos;
using ElementaryMathStudyWebsite.Contract.UseCases.IAppServices;
using ElementaryMathStudyWebsite.Services.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class IndexModel : PageModel
{
    private readonly IAppSubjectServices _subjectService;

    public IndexModel(IAppSubjectServices subjectService)
    {
        _subjectService = subjectService;
    }

    public List<Feature> Features { get; set; }
    public List<SubjectDTO> AvailableSubjects { get; set; }
    public List<Testimonial> Testimonials { get; set; }

    public async Task<IActionResult> OnGet()
    {
        // Mock data for features
        Features = new List<Feature>
        {
            new Feature { Title = "Interactive Lessons", Description = "Engage in fun and interactive lessons tailored for kids.", Icon = "📚" },
            new Feature { Title = "Progress Tracking", Description = "Monitor your learning progress with easy-to-read reports.", Icon = "📊" },
            new Feature { Title = "Parent Dashboard", Description = "Stay informed with detailed insights on your child's learning.", Icon = "👨‍👩‍👧" },
        };

        // Fetch available subjects
        AvailableSubjects = (await _subjectService.GetAllSubjectsAsync(-1, -1, false))
            .Items
            .OfType<SubjectDTO>()
            .ToList();

        // Mock data for testimonials
        Testimonials = new List<Testimonial>
        {
            new Testimonial { User = "Tran Van Tuan", Company = "Parent of a 3rd Grader", Text = "This platform has made learning so enjoyable for my child!", Image = "https://www.safeaustin.org/wp-content/uploads/2018/07/Feature_image_Parentingtips.png" },
            new Testimonial { User = "Nguyen Thi Thanh Mai", Company = "Parent of a 2nd Grader", Text = "I love how my child looks forward to lessons every day.", Image = "https://i.vietgiaitri.com/2019/9/19/ngam-loat-anh-phu-huynh-thoi-tre-nhan-sac-ngay-xua-chap-het-cac-app-chinh-anh-tho-photoshop-that-nghiep-nhu-choi-93434b.jpg" },
            new Testimonial { User = "Nguyen Nobel", Company = "Parent of a 4th Grader", Text = "The progress tracking feature is fantastic!", Image = "https://t.vietgiaitri.com/2019/08/9/dan-mang-khoe-anh-bo-dep-nhu-idol-hot-boy-va-rich-kid-doi-dau-voi-nhan-sac-thach-thuc-thoi-gian-la-day-9c2-250x180.jpg" },
        };

        return Page();
    }
}

public class Feature
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
}

public class Testimonial
{
    public string User { get; set; }
    public string Company { get; set; }
    public string Text { get; set; }
    public string Image { get; set; }
}
