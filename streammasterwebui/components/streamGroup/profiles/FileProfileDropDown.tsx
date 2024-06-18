import SMDropDown from '@components/sm/SMDropDown';
import useGetFileProfiles from '@lib/smAPI/Profiles/useGetFileProfiles';
import { FileOutputProfileDto, StreamGroupProfile } from '@lib/smAPI/smapiTypes';

import { ReactNode, useCallback, useMemo } from 'react';

interface FileProfileDropDownProps {
  readonly value: string;
  readonly onChange: (value: StreamGroupProfile) => void;
  readonly buttonDarkBackground?: boolean;
}

const FileProfileDropDown = ({ buttonDarkBackground = false, onChange, value }: FileProfileDropDownProps) => {
  const { data, isLoading } = useGetFileProfiles();

  const selectedFileProfile = useMemo(() => {
    if (!data) {
      return undefined;
    }

    return data.find((x) => x.Name === value);
  }, [data, value]);

  const itemTemplate = useCallback((option: FileOutputProfileDto): JSX.Element => {
    return <div className="text-xs text-container">{option?.Name ?? ''}</div>;
  }, []);

  const buttonTemplate = useMemo((): ReactNode => {
    return (
      <div className="sm-epg-selector">
        <div className="text-container" style={{ paddingLeft: '0.12rem' }}>
          {selectedFileProfile?.Name ?? ''}
        </div>
      </div>
    );
  }, [selectedFileProfile?.Name]);

  return (
    <SMDropDown
      isOverLayLoading={isLoading}
      buttonTemplate={buttonTemplate}
      buttonDarkBackground={buttonDarkBackground}
      contentWidthSize="2"
      data={data}
      dataKey="label"
      info=""
      itemTemplate={itemTemplate}
      label={buttonDarkBackground ? 'File Profile' : undefined}
      onChange={onChange}
      optionValue="label"
      scrollHeight="20vh"
      title="Profiles"
      value={selectedFileProfile}
    />
  );
};

export default FileProfileDropDown;
