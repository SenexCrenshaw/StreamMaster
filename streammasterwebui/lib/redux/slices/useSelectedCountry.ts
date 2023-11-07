import { useDispatch, useSelector } from 'react-redux';
import { type AppDispatch, type RootState } from '../store';
import { selectedCountryInternal } from './selectedCountrySlice';

export const useSelectedCountry = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setuseSelectedCountry = (newValue: string) => {
    dispatch(
      selectedCountryInternal({
        value: newValue,
        typename
      })
    );
  };

  const useSelectedCountry = useSelector((rootState: RootState) => rootState.selectedCountry[typename]);

  return { useSelectedCountry, setuseSelectedCountry };
};
