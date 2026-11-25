export interface ReleaseMetadata {
  version: string | null;
  tag: string | null;
  releaseDate: string | null;
  releaseUrl: string;
  installerUrl: string | null;
  zipUrl: string | null;
  checksumsUrl: string | null;
  releaseNotes: string;
}

export const release: ReleaseMetadata = {
  version: null,
  tag: null,
  releaseDate: null,
  releaseUrl: 'https://github.com/ZeroDayZ7/CyberCalc.NET9/releases',
  installerUrl: null,
  zipUrl: null,
  checksumsUrl: null,
  releaseNotes: ''
};
