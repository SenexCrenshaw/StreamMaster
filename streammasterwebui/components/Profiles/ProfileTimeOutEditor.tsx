import NumberEditorBodyTemplate from '@components/NumberEditorBodyTemplate';
import { FfmpegProfileDto, UpdateFfmpegProfileRequest } from '@lib/iptvApi';
import { UpdateFFMPEGProfile } from '@lib/smAPI/Profiles/ProfilesMutateAPI';

import React from 'react';

export interface ProfileTimeOutEditorProperties {
  readonly data: FfmpegProfileDto;
}

const ProfileTimeOutEditor = (props: ProfileTimeOutEditorProperties) => {
  const onUpdateFfmpegProfileDto = React.useCallback(
    async (timeOut: number) => {
      if (!timeOut || timeOut === 0) {
        return;
      }

      const data = {} as UpdateFfmpegProfileRequest;

      data.name = props.data.name;
      data.timeOut = timeOut;

      await UpdateFFMPEGProfile(data)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        });
    },
    [props.data]
  );

  if (props.data.name === undefined) {
    return <span className="sm-inputtext" />;
  }

  return (
    <NumberEditorBodyTemplate
      onChange={async (e) => {
        await onUpdateFfmpegProfileDto(e);
      }}
      value={props.data.timeout}
    />
  );
};

ProfileTimeOutEditor.displayName = 'Channel Number Editor';

export default React.memo(ProfileTimeOutEditor);
