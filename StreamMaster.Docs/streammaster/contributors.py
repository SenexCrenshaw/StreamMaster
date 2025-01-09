import requests

# Replace with your repository details
repo_owner = "SenexCrenshaw"
repo_name = "StreamMaster"
contributors_url = f"https://api.github.com/repos/{repo_owner}/{repo_name}/contributors"

response = requests.get(contributors_url)
contributors = response.json()

# Ensure SenexCrenshaw is the first contributor
contributors_sorted = sorted(contributors, key=lambda c: c["login"] != "SenexCrenshaw")

# Open the markdown file for writing in the docs\en directory
with open("docs\\en\\Contributors.md", "w", encoding="utf-8") as f:
    f.write("## Contributor Recognition 🌟\n\n")
    f.write("We appreciate every contribution, no matter how small! All contributors will be added to the StreamMaster Documentation Hall of Fame, featured below:\n\n")
    
    f.write('<div style="display: grid; grid-template-columns: repeat(auto-fill, minmax(120px, 1fr)); gap: 20px;">\n')

    avatar_size = 40  # Set avatar size

    for contributor in contributors_sorted:
        username = contributor["login"]

        # Skip the user 'semantic-release-bot'
        if username == "semantic-release-bot":
            continue

        profile_url = contributor["html_url"]
        avatar_url = f"{contributor['avatar_url']}&s={avatar_size}"  # Resize avatars

        # Special style for SenexCrenshaw with a trophy
        if username == "SenexCrenshaw":
            f.write(f'<div style="text-align: center;">')
            f.write(f'<img src="{avatar_url}" alt="{username}" style="border-radius: 50%; width: {avatar_size}px; height: {avatar_size}px;">')
            f.write(f'<br><a href="{profile_url}" style="white-space: nowrap;"><strong>🏆 {username}</strong></a>')
            f.write(f'</div>\n')
        else:
            # Normal contributor styling
            f.write(f'<div style="text-align: center;">')
            f.write(f'<img src="{avatar_url}" alt="{username}" style="border-radius: 50%; width: {avatar_size}px; height: {avatar_size}px;">')
            f.write(f'<br><a href="{profile_url}" style="white-space: nowrap;">{username}</a>')
            f.write(f'</div>\n')

    f.write('</div>\n')

print("Contributors list generated in docs\\en\\Contributors.md")
