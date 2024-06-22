import SearchButton from '@components/buttons/SearchButton';
import StringEditor from '@components/inputs/StringEditor';
import { useSelectedCountry } from '@lib/redux/hooks/selectedCountry';
import { useSelectedPostalCode } from '@lib/redux/hooks/selectedPostalCode';
import { useSMContext } from '@lib/signalr/SMProvider';

import useGetAvailableCountries from '@lib/smAPI/SchedulesDirect/useGetAvailableCountries';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsCommands';
import { Country, CountryData, UpdateSettingRequest } from '@lib/smAPI/smapiTypes';

import { Dropdown } from 'primereact/dropdown';
import React from 'react';

interface SchedulesDirectCountrySelectorProperties {
  readonly onChange?: (value: string) => void;
}

const SchedulesDirectCountrySelector = (props: SchedulesDirectCountrySelectorProperties) => {
  const { settings } = useSMContext();
  const { selectedCountry, setSelectedCountry } = useSelectedCountry('Country');
  const { selectedPostalCode, setSelectedPostalCode } = useSelectedPostalCode('PostalCode');

  // const [countryValue, setCountryValue] = useState<string | undefined>();
  // const [postalCodeValue, setPostalCodeValue] = useState<string | undefined>();

  const getCountriesQuery = useGetAvailableCountries();

  React.useEffect(() => {
    // console.log('sdCountry', setting.data?.sdSettings?.sdCountry, selectedCountry);
    // if (setting.data?.sdSettings?.sdCountry !== undefined) {
    //   if (setting.data?.sdSettings?.sdCountry !== selectedCountry) {
    //     setSelectedCountry(setting.data?.sdSettings?.sdCountry);
    //   }
    //   if (setting.data?.sdSettings?.sdCountry !== countryValue) {
    //     setCountryValue(setting.data?.sdSettings?.sdCountry);
    //   }
    // }

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

  return (
    <div className="flex grid col-12 pl-1 justify-content-start align-items-center p-0 m-0">
      <div className="flex col-6 p-0 pr-2">
        <Dropdown
          className="bordered-text w-full"
          filter
          onChange={(e) => {
            setSelectedCountry(e.value);
            props.onChange?.(e.value);
          }}
          options={options}
          placeholder="Country"
          style={{
            backgroundColor: 'var(--mask-bg)',
            overflow: 'hidden',
            textOverflow: 'ellipsis',
            whiteSpace: 'nowrap'
          }}
          value={selectedCountry}
        />
      </div>
      <div className="flex col-6 p-0">
        <div className="flex col-6 p-0">
          <StringEditor
            disableDebounce
            placeholder="Postal Code"
            onChange={(e) => {
              if (e !== undefined) {
                setSelectedPostalCode(e);
              }
            }}
            value={selectedPostalCode ?? ''}
          />
        </div>
        <div className="flex col-2 pt-2 p-0 pr-3">
          <SearchButton
            tooltip="Go"
            onClick={() => {
              // console.log('PostalCode', postalCodeValue);

              if (!selectedPostalCode || !selectedCountry) {
                return;
              }

              // console.log('UpdateSetting', postalCodeValue, countryValue);
              // console.log('UpdateSetting', setting.data.sdSettings?.sdPostalCode, setting.data.sdSettings?.sdCountry);

              // if (setting.data.sdSettings?.sdPostalCode === postalCodeValue && setting.data.sdSettings?.sdCountry === countryValue) {
              //   return;
              // }

              // setSelectedCountry(countryValue);
              // setSelectedPostalCode(postalCodeValue);

              const newData: UpdateSettingRequest = { parameters: { SDSettings: { SDCountry: selectedCountry, SDPostalCode: selectedPostalCode } } };

              UpdateSetting(newData)
                .then(() => {})
                .catch(() => {});
            }}
          />
        </div>
      </div>
    </div>
  );
};

SchedulesDirectCountrySelector.displayName = 'SchedulesDirectCountrySelector';

export default React.memo(SchedulesDirectCountrySelector);
