import SearchButton from '@components/buttons/SearchButton';
import StringEditor from '@components/inputs/StringEditor';
import SMDropDown from '@components/sm/SMDropDown';
import { useSMContext } from '@lib/context/SMProvider';
import { useSelectedCountry } from '@lib/redux/hooks/selectedCountry';
import { useSelectedPostalCode } from '@lib/redux/hooks/selectedPostalCode';

import useGetAvailableCountries from '@lib/smAPI/SchedulesDirect/useGetAvailableCountries';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsCommands';
import { Country, CountryData, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

import React, { useMemo } from 'react';

interface SchedulesDirectCountrySelectorProperties {
  readonly onChange?: (data: { Country: string; PostalCode: string }) => void;
}

const SchedulesDirectCountrySelector = (props: SchedulesDirectCountrySelectorProperties) => {
  const { settings } = useSMContext();
  const { selectedCountry, setSelectedCountry } = useSelectedCountry('Country');
  const { selectedPostalCode, setSelectedPostalCode } = useSelectedPostalCode('PostalCode');

  const getCountriesQuery = useGetAvailableCountries();

  React.useEffect(() => {
    if ((selectedCountry === undefined || selectedCountry === '') && settings.SDSettings?.SDCountry !== undefined) {
      setSelectedCountry(settings.SDSettings?.SDCountry);
    }

    //console.log('sdPostalCode', setting.data?.sdSettings?.sdPostalCode, selectedPostalCode);
    if ((selectedPostalCode === undefined || selectedPostalCode === '') && settings.SDSettings?.SDPostalCode !== undefined) {
      setSelectedPostalCode(settings.SDSettings?.SDPostalCode);
    }
  }, [settings.SDSettings?.SDCountry, settings.SDSettings?.SDPostalCode, selectedCountry, selectedPostalCode, setSelectedCountry, setSelectedPostalCode]);

  // interface Country {
  //   ShortName: string;
  //   FullName?: string;
  // }

  interface CountryOption {
    label: string;
    value: string;
  }

  const options: CountryOption[] = React.useMemo(() => {
    if (!getCountriesQuery.data) return [];

    const countries: CountryOption[] = [];

    Object.values(getCountriesQuery.data as CountryData[]).forEach((continentCountry: CountryData) => {
      if (continentCountry.Countries === undefined) return;

      continentCountry.Countries.filter((c): c is Country => c.ShortName !== undefined && c.ShortName.trim() !== '')?.forEach((country: Country) => {
        countries.push({
          label: country.FullName || 'Unknown Country',
          value: country.ShortName ?? ''
        });
      });
    });

    return countries.sort((a, b) => a.label.localeCompare(b.label));
  }, [getCountriesQuery.data]);

  const buttonTemplate = useMemo(() => {
    const found = options.find((o) => o.value === selectedCountry);
    if (found) {
      return <div>{found.label}</div>;
    }
    return <div>{selectedCountry}</div>;
  }, [options, selectedCountry]);

  return (
    <div className="flex w-full justify-content-between align-items-center p-0 m-0 ">
      <div className="sm-w-8">
        <SMDropDown
          buttonDarkBackground
          buttonTemplate={buttonTemplate}
          contentWidthSize="2"
          data={options}
          dataKey="value"
          filter
          filterBy="label"
          itemTemplate={(option: CountryOption) => <div className="text-content">{option.label}</div>}
          onChange={(e) => {
            setSelectedCountry(e.value);
          }}
          placement="bottom"
          value={selectedCountry}
        />
      </div>
      <div className="flex sm-w-4 justify-content-between align-items-center p-0 m-0 ">
        <div className="w-10">
          <StringEditor
            darkBackGround
            disableDebounce
            placeholder="Postal Code"
            onChange={(e) => {
              if (e === null || e === undefined || e === '') return;
              setSelectedPostalCode(e);
            }}
            value={selectedPostalCode ?? ''}
          />
        </div>
        <div className="w-2">
          <SearchButton
            tooltip="Go"
            onClick={() => {
              if (selectedCountry === null || selectedCountry === undefined || selectedCountry === '') return;
              if (selectedPostalCode === null || selectedPostalCode === undefined || selectedPostalCode === '') return;

              const newData: UpdateSettingRequest = { Parameters: { SDSettings: { SDCountry: selectedCountry, SDPostalCode: selectedPostalCode } } };

              UpdateSetting(newData)
                .then(() => {})
                .catch(() => {})
                .finally(() => {
                  props.onChange?.({ Country: selectedCountry, PostalCode: selectedPostalCode });
                });
            }}
          />
        </div>
      </div>
    </div>
  );
};

SchedulesDirectCountrySelector.displayName = 'SchedulesDirectCountrySelector';

export default React.memo(SchedulesDirectCountrySelector);
