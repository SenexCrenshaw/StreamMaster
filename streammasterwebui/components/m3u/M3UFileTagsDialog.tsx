import React from 'react';
import M3UFileTags from './M3UFileTags';

export interface M3UFileTagsDialogProperties {
  m3uFileDto: M3UFileDto;
}

const M3UFileTagsDialog = ({ m3uFileDto }: M3UFileTagsDialogProperties) => {
  const [M3UFilesUpdateM3UFileMutation] = useM3UFilesUpdateM3UFileMutation();

  const updateM3U = async (vodTags: string[]) => {
    const updateM3UFileRequest = {} as UpdateM3UFileRequest;

    updateM3UFileRequest.id = m3uFileDto.id;
    updateM3UFileRequest.vodTags = vodTags;

    await M3UFilesUpdateM3UFileMutation(updateM3UFileRequest)
      .then(() => {})
      .catch(() => {
        console.log(`Error Updating M3U`);
      });
  };

  return <M3UFileTags m3uFileDto={m3uFileDto} onChange={(e) => updateM3U(e)} />;
};

M3UFileTagsDialog.displayName = 'M3UFileDialog';

export default React.memo(M3UFileTagsDialog);
