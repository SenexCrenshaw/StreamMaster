import StringEditor from '@components/inputs/StringEditor';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { StreamGroupProfile } from '@lib/smAPI/smapiTypes';
import { useCallback } from 'react';

export interface CommandProfileColumnConfigProps {
  readonly field?: string;
  readonly header?: string;
  readonly width?: number | string;
}

interface IntCommandProfileColumnConfigProps {
  readonly field: string;
  readonly header: string;
  readonly width?: number | string;
}

export const useStreamGroupProfileColumnConfig = ({ field, header, width = 80 }: IntCommandProfileColumnConfigProps) => {
  // function updateProfile<T extends keyof UpdateCommandProfileRequest>(profile: UpdateCommandProfileRequest, key: T, value: UpdateCommandProfileRequest[T]) {
  //   if (value !== undefined && value !== null) {
  //     profile[key] = value;
  //   }
  // }

  // const update = useCallback((request: UpdateCommandProfileRequest) => {
  //   console.log('update', request);

  //   // UpdateCommandProfile(request)
  //   //   .then((res) => {})
  //   //   .catch((error) => {
  //   //     console.log('error', error);
  //   //   })
  //   //   .finally();
  // }, []);

  const bodyTemplate = useCallback(
    (videoOutputProfile: StreamGroupProfile) => {
      var key = field as keyof StreamGroupProfile;
      let value = videoOutputProfile[key] as string;

      return (
        <StringEditor
          onSave={(e) => {
            if (e !== undefined) {
              // const updateCommandProfileRequest = { Name: videoOutputProfile.Name } as UpdateCommandProfileRequest;
              // var key = field as keyof UpdateCommandProfileRequest;
              // updateProfile(updateCommandProfileRequest, key, e);
              // update(updateCommandProfileRequest);
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
