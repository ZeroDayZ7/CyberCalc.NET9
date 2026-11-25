import { writeFile, mkdir } from 'node:fs/promises';

const repository = process.env.GITHUB_REPOSITORY ?? 'ZeroDayZ7/CyberCalc.NET9';
const token = process.env.GITHUB_TOKEN;
const tagArgument = process.argv.find((argument) => argument.startsWith('--tag='))?.slice(6);
const outputArgument = process.argv.find((argument) => argument.startsWith('--output='))?.slice(9);
const output = outputArgument ?? 'src/data/release.json';

const fallback = {
  version: null,
  tag: null,
  releaseDate: null,
  releaseUrl: `https://github.com/${repository}/releases`,
  installerUrl: null,
  zipUrl: null,
  checksumsUrl: null,
  releaseNotes: ''
};

async function fetchRelease() {
  const endpoint = tagArgument
    ? `https://api.github.com/repos/${repository}/releases/tags/${encodeURIComponent(tagArgument)}`
    : `https://api.github.com/repos/${repository}/releases/latest`;

  const headers = { Accept: 'application/vnd.github+json', 'X-GitHub-Api-Version': '2022-11-28' };
  if (token) headers.Authorization = `Bearer ${token}`;

  const response = await fetch(endpoint, { headers });
  if (response.status === 404) return null;
  if (!response.ok) throw new Error(`GitHub API returned ${response.status}: ${await response.text()}`);
  return response.json();
}

try {
  const release = await fetchRelease();
  if (!release) {
    await mkdir(output.split('/').slice(0, -1).join('/') || '.', { recursive: true });
    await writeFile(output, `${JSON.stringify(fallback, null, 2)}\n`, 'utf8');
  } else {
    const assets = new Map(release.assets?.map((asset) => [asset.name, asset]) ?? []);
    const version = (release.tag_name ?? '').replace(/^v/, '') || null;
    const installer = [...assets.values()].find((asset) => /^CyberCalc-Setup-.+-x64\.exe$/.test(asset.name));
    const zip = [...assets.values()].find((asset) => /^CyberCalc-.+-win-x64\.zip$/.test(asset.name));
    const checksums = [...assets.values()].find((asset) => /^CyberCalc-.+-SHA256SUMS\.txt$/.test(asset.name));

    const metadata = {
      version,
      tag: release.tag_name ?? null,
      releaseDate: release.published_at ?? null,
      releaseUrl: release.html_url,
      installerUrl: installer?.browser_download_url ?? null,
      zipUrl: zip?.browser_download_url ?? null,
      checksumsUrl: checksums?.browser_download_url ?? null,
      releaseNotes: release.body ?? ''
    };

    await mkdir(output.split('/').slice(0, -1).join('/') || '.', { recursive: true });
    await writeFile(output, `${JSON.stringify(metadata, null, 2)}\n`, 'utf8');
  }
} catch (error) {
  await mkdir(output.split('/').slice(0, -1).join('/') || '.', { recursive: true });
  await writeFile(output, `${JSON.stringify(fallback, null, 2)}\n`, 'utf8');
  console.warn(`Release metadata unavailable; using fallback: ${error instanceof Error ? error.message : String(error)}`);
}
