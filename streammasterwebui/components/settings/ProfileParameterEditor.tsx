import StringEditorBodyTemplate from '@components/StringEditorBodyTemplate';
import { FfmpegProfileDto, UpdateFfmpegProfileRequest } from '@lib/iptvApi';
import { UpdateFFMPEGProfile } from '@lib/smAPI/Settings/SettingsMutateAPI';
import React from 'react';

export interface ProfileParameterEditorProperties {
  readonly data: FfmpegProfileDto;
}

const ProfileParameterEditor = (props: ProfileParameterEditorProperties) => {
  const onUpdateFfmpegProfileDto = React.useCallback(
    async (parameters: string) => {
      if (!parameters || parameters === '') {
        return;
      }

      const data = {} as UpdateFfmpegProfileRequest;

      data.name = props.data.name;
      data.parameters = parameters;

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
    <StringEditorBodyTemplate
      onChange={async (e) => {
        await onUpdateFfmpegProfileDto(e);
      }}
      value={props.data.parameters}
    />
  );
};

ProfileParameterEditor.displayName = 'Channel Number Editor';

export default React.memo(ProfileParameterEditor);
