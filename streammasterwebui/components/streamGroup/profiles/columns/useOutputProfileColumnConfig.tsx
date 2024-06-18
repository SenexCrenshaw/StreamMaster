import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { Logger } from '@lib/common/logger';
import { OutputProfileDto } from '@lib/smAPI/smapiTypes';
import { useCallback } from 'react';
import FileProfileValueDropDown from './OutputProfileValueDropDown';

export interface OutputProfileColumnConfigProps {
  readonly field?: string;
  readonly header?: string;
  readonly width?: number;
}

interface IntOutputProfileColumnConfigProps {
  readonly field: string;
  readonly header: string;
  readonly width?: number;
}

export const useOutputProfileColumnConfig = ({ field, header, width = 80 }: IntOutputProfileColumnConfigProps) => {
  const bodyTemplate = useCallback(
    (fileOutputProfile: OutputProfileDto) => {
      var key = field as keyof OutputProfileDto;
      let value = fileOutputProfile[key] as string;

      // if (fileOutputProfile.IsReadOnly === true) {
      //   return (
      //     <div className="sm-epg-selector">
      //       <div className="text-container pl-1">{value}</div>
      //     </div>
      //   );
      // }

      Logger.debug('value', key, value, field);
      return <FileProfileValueDropDown header={header} value={value} field={field} name={fileOutputProfile.Name} />;
    },
    [field, header]
  );

  const columnConfig: ColumnMeta = {
    bodyTemplate: bodyTemplate,
    field: field,
    header: header,
    width: width
  };

  return columnConfig;
};
