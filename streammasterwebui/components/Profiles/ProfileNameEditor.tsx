import StringEditorBodyTemplate from '@components/inputs/StringEditorBodyTemplate';

import React, { useEffect, useState } from 'react';

export interface ProfileNameEditorProperties {
  readonly data: FfmpegProfileDto;
}

const ProfileNameEditor = (props: ProfileNameEditorProperties) => {
  const [oldName, setOldName] = useState<string | undefined>(undefined);

  useEffect(() => {
    if (oldName === undefined && props.data.name !== undefined) {
      setOldName(props.data.name);
    }
  }, [oldName, props.data.name]);

  const onUpdateFfmpegProfileDto = React.useCallback(
    async (name: string) => {
      if (!name || name === '' || oldName === name) {
        return;
      }

      const data = {} as UpdateFfmpegProfileRequest;

      data.name = oldName;
      data.newName = name;

      await UpdateFFMPEGProfile(data)
        .then(() => {
          setOldName(name);
        })
        .catch((error) => {
          console.error(error);
        });
    },
    [oldName]
  );

  if (props.data.name === undefined) {
    return <span className="sm-inputtext" />;
  }

  return (
    <StringEditorBodyTemplate
      onChange={async (e) => {
        await onUpdateFfmpegProfileDto(e);
      }}
      value={props.data.name}
    />
  );
};

ProfileNameEditor.displayName = 'Channel Number Editor';

export default React.memo(ProfileNameEditor);
