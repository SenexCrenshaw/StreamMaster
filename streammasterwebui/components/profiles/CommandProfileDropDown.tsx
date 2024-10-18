import SMDropDown from '@components/sm/SMDropDown';
import { Logger } from '@lib/common/logger';
import useGetCommandProfiles from '@lib/smAPI/Profiles/useGetCommandProfiles';
import { CommandProfileDto } from '@lib/smAPI/smapiTypes';
import { ReactNode, useCallback, useMemo } from 'react';

interface CommandProfileDropDownProps {
  readonly buttonDarkBackground?: boolean;
  readonly label?: string;
  readonly value: string;
  readonly onChange: (value: any) => void;
}

const CommandProfileDropDown = ({ buttonDarkBackground = false, onChange, label = 'Command Profile', value }: CommandProfileDropDownProps) => {
  const { data, isLoading } = useGetCommandProfiles();

  const dataSource = useMemo((): CommandProfileDto[] => {
    if (!data) {
      return [];
    }

    if (value) {
      const ret = data.filter((x) => x.ProfileName !== 'Use SG');
      return ret;
    }

    return data;
  }, [data, value]);

  const selectedCommandProfile = useMemo(() => {
    if (!dataSource || !value) {
      return undefined;
    }

    return dataSource.find((x) => x.ProfileName === value);
  }, [dataSource, value]);

  const itemTemplate = useCallback((option: CommandProfileDto): JSX.Element => {
    return <div className="text-xs text-container">{option?.ProfileName ?? ''}</div>;
  }, []);

  const buttonTemplate = useMemo((): ReactNode => {
    Logger.debug('CommandProfileDropDown', { selectedCommandProfile });
    return (
      <div className="text-container" style={{ paddingLeft: '0.24rem' }}>
        {selectedCommandProfile?.ProfileName ?? ''}
      </div>
    );
  }, [selectedCommandProfile]);

  return (
    <SMDropDown
      buttonDarkBackground={buttonDarkBackground}
      buttonTemplate={buttonTemplate}
      contentWidthSize="2"
      data={dataSource}
      dataKey="ProfileName"
      info=""
      isOverLayLoading={isLoading}
      itemTemplate={itemTemplate}
      label={buttonDarkBackground ? label : undefined}
      onChange={onChange}
      propertyToMatch="ProfileName"
      scrollHeight="20vh"
      title="Profiles"
      value={selectedCommandProfile}
      zIndex={12}
    />
  );
};

export default CommandProfileDropDown;
