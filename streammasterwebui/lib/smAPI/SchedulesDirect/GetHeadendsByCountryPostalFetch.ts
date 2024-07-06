import { GetHeadendsByCountryPostal } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { GetHeadendsByCountryPostalRequest } from '../smapiTypes';
import { isSkipToken } from '@lib/common/isSkipToken';
import { Logger } from '@lib/common/logger';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetHeadendsByCountryPostal = createAsyncThunk('cache/getGetHeadendsByCountryPostal', async (param: GetHeadendsByCountryPostalRequest, thunkAPI) => {
  try {
    if (isSkipToken(param))
    {
        Logger.error('Skipping GetEPGFilePreviewById');
        return undefined;
    }
    Logger.debug('Fetching GetHeadendsByCountryPostal');
    const response = await GetHeadendsByCountryPostal(param);
    return {param: param, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


