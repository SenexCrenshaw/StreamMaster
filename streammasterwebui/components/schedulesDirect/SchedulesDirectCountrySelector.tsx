import SearchButton from '@components/buttons/SearchButton';
import TextInput from '@components/inputs/TextInput';
import { CountryData, SchedulesDirectGetAvailableCountriesApiResponse, UpdateSettingRequest, useSchedulesDirectGetAvailableCountriesQuery } from '@lib/iptvApi';
import { useSelectedCountry } from '@lib/redux/slices/selectedCountrySlice';
import { useSelectedPostalCode } from '@lib/redux/slices/selectedPostalCodeSlice';
import { UpdateSetting } from '@lib/smAPI/Settings/SettingsMutateAPI';
import useSettings from '@lib/useSettings';
import { Dropdown } from 'primereact/dropdown';
import React from 'react';

interface SchedulesDirectCountrySelectorProperties {
  readonly onChange?: (value: string) => void;
}

const SchedulesDirectCountrySelector = (props: SchedulesDirectCountrySelectorProperties) => {
  const setting = useSettings();
  const { selectedCountry, setSelectedCountry } = useSelectedCountry('Country');
  const { selectedPostalCode, setSelectedPostalCode } = useSelectedPostalCode('PostalCode');

  // const [countryValue, setCountryValue] = useState<string | undefined>();
  // const [postalCodeValue, setPostalCodeValue] = useState<string | undefined>();

  const getCountriesQuery = useSchedulesDirectGetAvailableCountriesQuery();

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

    if ((selectedCountry === undefined || selectedCountry === '') && setting.data?.sdSettings?.sdCountry !== undefined) {
      setSelectedCountry(setting.data?.sdSettings?.sdCountry);
    }

    //console.log('sdPostalCode', setting.data?.sdSettings?.sdPostalCode, selectedPostalCode);
    if ((selectedPostalCode === undefined || selectedPostalCode === '') && setting.data?.sdSettings?.sdPostalCode !== undefined) {
      setSelectedPostalCode(setting.data?.sdSettings?.sdPostalCode);
    }
  }, [
    selectedCountry,
    selectedPostalCode,
    setSelectedCountry,
    setSelectedPostalCode,
    setting.data?.sdSettings?.sdCountry,
    setting.data?.sdSettings?.sdPostalCode
  ]);

  interface Country {
    shortName: string;
    fullName?: string;
  }

  interface CountryOption {
    label: string;
    value: string;
  }

  const options: CountryOption[] = React.useMemo(() => {
    if (!getCountriesQuery.data) return [];

    const countries: CountryOption[] = [];

    Object.values(getCountriesQuery.data as SchedulesDirectGetAvailableCountriesApiResponse).forEach((continentCountry: CountryData) => {
      if (continentCountry.countries === undefined) return;

      continentCountry.countries
        .filter((c): c is Country => c.shortName !== undefined && c.shortName.trim() !== '')
        ?.forEach((country: Country) => {
          countries.push({
            label: country.fullName || 'Unknown Country',
            value: country.shortName ?? ''
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
          <TextInput
            placeHolder="Postal Code"
            onChange={(e) => {
              setSelectedPostalCode(e);
            }}
            value={selectedPostalCode}
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

              const newData: UpdateSettingRequest = { sdSettings: { sdPostalCode: selectedPostalCode, sdCountry: selectedCountry } };

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
