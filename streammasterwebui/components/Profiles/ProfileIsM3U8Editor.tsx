import { getTopToolOptions } from '@lib/common/common';

import { Checkbox, CheckboxChangeEvent } from 'primereact/checkbox';
import React, { useEffect } from 'react';

export interface ProfileIsM3U8EditorProperties {
  readonly data: FfmpegProfileDto;
}

const ProfileIsM3U8Editor = (props: ProfileIsM3U8EditorProperties) => {
  const [checked, setChecked] = React.useState<boolean | undefined>(undefined);

  useEffect(() => {
    if (checked === undefined && props.data.isM3U8 !== undefined) {
      setChecked(props.data.isM3U8);
    }
  }, [checked, props.data.isM3U8, props.data.name]);

  const onUpdateFfmpegProfileDto = React.useCallback(
    async (isM3U8: boolean) => {
      const data = {} as UpdateFfmpegProfileRequest;

      data.name = props.data.name;
      data.isM3U8 = isM3U8;

      await UpdateFFMPEGProfile(data)
        .then(() => {
          setChecked(isM3U8);
        })
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
    <Checkbox
      checked={checked ?? false}
      onChange={async (e: CheckboxChangeEvent) => {
        await onUpdateFfmpegProfileDto(e.checked ?? false);
      }}
      tooltip="Is M3U8"
      tooltipOptions={getTopToolOptions}
    />
  );
};

ProfileIsM3U8Editor.displayName = 'Channel Number Editor';

export default React.memo(ProfileIsM3U8Editor);
