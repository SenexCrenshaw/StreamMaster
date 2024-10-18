import { UpdateM3UFile } from '@lib/smAPI/M3UFiles/M3UFilesCommands';
import { M3UFileDto, UpdateM3UFileRequest } from '@lib/smAPI/smapiTypes';
import React from 'react';
import M3UFileTags from './M3UFileTags';

export interface M3UFileTagsDialogProperties {
  m3uFileDto: M3UFileDto;
}

const M3UFileTagsDialog = ({ m3uFileDto }: M3UFileTagsDialogProperties) => {
  const updateM3U = async (vodTags: string[]) => {
    const updateM3UFileRequest = {} as UpdateM3UFileRequest;

    updateM3UFileRequest.Id = m3uFileDto.Id;
    updateM3UFileRequest.VODTags = vodTags;

    await UpdateM3UFile(updateM3UFileRequest)
      .then(() => {})
      .catch(() => {
        console.log(`Error Updating M3U`);
      });
  };

  return <M3UFileTags m3uFileDto={m3uFileDto} onChange={(e) => updateM3U(e)} />;
};

M3UFileTagsDialog.displayName = 'M3UFileDialog';

export default React.memo(M3UFileTagsDialog);
