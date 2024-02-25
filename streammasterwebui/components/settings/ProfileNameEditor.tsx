import StringEditorBodyTemplate from '@components/StringEditorBodyTemplate';
import { FfmpegProfileDto } from '@lib/iptvApi';
import React from 'react';

export interface ProfileNameEditorProperties {
  readonly data: FfmpegProfileDto;
}

const ProfileNameEditor = (props: ProfileNameEditorProperties) => {
  const onUpdateFfmpegProfileDto = React.useCallback(
    async (name: string) => {
      if (!name || name === '' || props.data.name === name) {
        return;
      }

      // const data = {} as UpdateVideoStreamRequest;

      // data.id = props.data.id;
      // data.tvg_name = name;

      // await UpdateVideoStream(data)
      //   .then(() => {})
      //   .catch((error) => {
      //     console.error(error);
      //   });
    },
    [props.data.name]
  );

  if (props.data.name === undefined) {
    return <span className="sm-inputtext" />;
  }

  return (
    <StringEditorBodyTemplate
      onChange={async (e) => {
        await onUpdateFfmpegProfileDto(e);
      }}
      resetValue={props.data.name}
      value={props.data.name}
    />
  );
};

ProfileNameEditor.displayName = 'Channel Number Editor';

export default React.memo(ProfileNameEditor);
