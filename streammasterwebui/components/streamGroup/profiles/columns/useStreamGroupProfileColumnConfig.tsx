import StringEditor from '@components/inputs/StringEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { StreamGroupProfile, UpdateVideoProfileRequest } from '@lib/smAPI/smapiTypes';
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

export const useStreamGroupProfileColumnConfig = ({ field, header, width = 80 }: IntVideoProfileColumnConfigProps) => {
  function updateProfile<T extends keyof UpdateVideoProfileRequest>(profile: UpdateVideoProfileRequest, key: T, value: UpdateVideoProfileRequest[T]) {
    if (value !== undefined && value !== null) {
      profile[key] = value;
    }
  }

  const update = useCallback((request: UpdateVideoProfileRequest) => {
    console.log('update', request);

    // UpdateVideoProfile(request)
    //   .then((res) => {})
    //   .catch((error) => {
    //     console.log('error', error);
    //   })
    //   .finally();
  }, []);

  const bodyTemplate = useCallback(
    (videoOutputProfile: StreamGroupProfile) => {
      var key = field as keyof StreamGroupProfile;
      let value = videoOutputProfile[key] as string;

      return (
        <StringEditor
          onSave={(e) => {
            if (e !== undefined) {
              // const updateVideoProfileRequest = { Name: videoOutputProfile.Name } as UpdateVideoProfileRequest;
              // var key = field as keyof UpdateVideoProfileRequest;
              // updateProfile(updateVideoProfileRequest, key, e);
              // update(updateVideoProfileRequest);
            }
          }}
          value={value}
        />
      );
    },
    [field]
  );

  const columnConfig: ColumnMeta = {
    bodyTemplate: bodyTemplate,
    field: field,
    header: header,
    width: width
  };

  return columnConfig;
};
