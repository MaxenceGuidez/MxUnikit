using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace MxUnikit.Tests.Editor
{
    public class ProjectReferenceValidationTests
    {
        [Test]
        public void ValidateAllPrefabs_NoMissingRequiredFields()
        {
            List<ValidationException> exceptions = PrefabScanner.ValidateAllPrefabs();

            if (exceptions.Count > 0)
            {
                StringBuilder errorMessage = new StringBuilder();
                errorMessage.AppendLine($"Found {exceptions.Count} missing required field(s) in prefabs:");

                foreach (ValidationException ex in exceptions)
                {
                    errorMessage.AppendLine($"  - {ex.Message}");
                }

                Assert.Fail(errorMessage.ToString());
            }

            Assert.Pass("Validated all prefabs successfully. No missing required fields.");
        }

        [Test]
        public void ValidateAllScenes_NoMissingRequiredFields()
        {
            List<ValidationException> exceptions = SceneScanner.ValidateAllScenes();

            if (exceptions.Count > 0)
            {
                StringBuilder errorMessage = new StringBuilder();
                errorMessage.AppendLine($"Found {exceptions.Count} missing required field(s) in scenes:");

                foreach (ValidationException ex in exceptions)
                {
                    errorMessage.AppendLine($"  - {ex.Message}");
                }

                Assert.Fail(errorMessage.ToString());
            }

            Assert.Pass("Validated all scenes successfully. No missing required fields.");
        }
    }
}
