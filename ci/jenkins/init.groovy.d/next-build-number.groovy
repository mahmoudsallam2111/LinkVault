// Docker Hub already holds numeric tags from Azure Pipelines. Start Jenkins'
// build counter after them so new images never overwrite old tags and
// Argo CD Image Updater keeps seeing increasing numbers.
import jenkins.model.Jenkins

def wanted = (System.getenv('NEXT_BUILD_NUMBER') ?: '0') as int
def job = Jenkins.get().getItemByFullName('linkvault')

if (job == null) {
  println "[linkvault] job not found yet; next build number not changed"
} else if (job.nextBuildNumber < wanted) {
  job.updateNextBuildNumber(wanted)
  println "[linkvault] next build number set to ${wanted}"
}
