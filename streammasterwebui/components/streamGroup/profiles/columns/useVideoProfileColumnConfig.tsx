import StringEditor from '@components/inputs/StringEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { UpdateVideoProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { UpdateVideoProfileRequest, VideoOutputProfileDto } from '@lib/smAPI/smapiTypes';
import { useCallback } from 'react';

export interface VideoProfileColumnConfigProps {
  readonly field?: string;
  readonly header?: string;
  readonly width?: number | string;
}

interface IntVideoProfileColumnConfigProps {
  readonly field: string;
  readonly header: string;
  readonly width?: number | string;
}

export const useVideoProfileColumnConfig = ({ field, header, width = 80 }: IntVideoProfileColumnConfigProps) => {
  function updateProfile<T extends keyof UpdateVideoProfileRequest>(profile: UpdateVideoProfileRequest, key: T, value: UpdateVideoProfileRequest[T]) {
    if (value !== undefined && value !== null) {
      profile[key] = value;
    }
  }

  const update = useCallback((request: UpdateVideoProfileRequest) => {
    console.log('update', request);

    UpdateVideoProfile(request)
      .then((res) => {})
      .catch((error) => {
        console.log('error', error);
      })
      .finally();
  }, []);

  const bodyTemplate = useCallback(
    (videoOutputProfile: VideoOutputProfileDto) => {
      var key = field as keyof VideoOutputProfileDto;
      let value = videoOutputProfile[key] as string;

      if (videoOutputProfile.IsReadOnly === true) {
        return (
          <div className="sm-epg-selector">
            <div className="text-container pl-1">{value}</div>
          </div>
        );
      }

      return (
        <StringEditor
          onSave={(e) => {
            if (e !== undefined) {
              const updateVideoProfileRequest = { Name: videoOutputProfile.Name } as UpdateVideoProfileRequest;
              var key = field as keyof UpdateVideoProfileRequest;
              updateProfile(updateVideoProfileRequest, key, e);

              update(updateVideoProfileRequest);
            }
          }}
          value={value}
        />
      );
    },
    [field, update]
  );

  const columnConfig: ColumnMeta = {
    bodyTemplate: bodyTemplate,
    field: field,
    header: header,
    width: width
  };

  return columnConfig;
};
