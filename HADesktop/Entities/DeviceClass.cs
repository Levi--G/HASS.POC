using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HADesktop.Entities
{
    internal enum DeviceClass
    {
        None,// Generic sensor. This is the default and doesn’t need to be set.
        absolute_humidity,// Absolute humidity in g/m³, mg/m³.
        apparent_power,// Apparent power in mVA, VA or kVA.
        aqi,// Air Quality Index (unitless).
        area,// Area in m², cm², km², mm², in², ft², yd², mi², ac, ha
        atmospheric_pressure,// Atmospheric pressure in cbar, bar, hPa, mmHg, inHg, kPa, mbar, Pa or psi
        battery,// Percentage of battery that is left in %
        blood_glucose_concentration,// Blood glucose concentration in mg/dL, mmol/L
        carbon_dioxide,// Carbon Dioxide (CO₂) concentration in ppm
        carbon_monoxide,// Carbon Monoxide (CO) concentration in ppb, ppm, µg/m³, mg/m³
        current,// Current in A, mA
        data_rate,// Data rate in bit/s, kbit/s, Mbit/s, Gbit/s, B/s, kB/s, MB/s, GB/s, KiB/s, MiB/s or GiB/s
        data_size,// Data size in bit, kbit, Mbit, Gbit, B, kB, MB, GB, TB, PB, EB, ZB, YB, KiB, MiB, GiB, TiB, PiB, EiB, ZiB or YiB
        date,// Date string (ISO 8601)
        distance,// Generic distance in km, m, cm, mm, mi, nmi, yd, or in
        duration,// Duration in d, h, min, s, ms, or µs
        energy,// Energy in J, kJ, MJ, GJ, mWh, Wh, kWh, MWh, GWh, TWh, cal, kcal, Mcal, or Gcal
        energy_distance,// Energy per distance in kWh/100km, Wh/km, mi/kWh, or km/kWh.
        energy_storage,// Stored energy in J, kJ, MJ, GJ, mWh, Wh, kWh, MWh, GWh, TWh, cal, kcal, Mcal, or Gcal
        @enum,// Has a limited set of (non-numeric) states
        frequency,// Frequency in Hz, kHz, MHz, or GHz
        gas,// Gas volume in L, m³, ft³, CCF, or MCF
        humidity,// Percentage of humidity in the air in %
        illuminance,// The current light level in lx
        irradiance,// Irradiance in W/m² or BTU/(h⋅ft²)
        moisture,// Percentage of water in a substance in %
        monetary,// The monetary value (ISO 4217)
        nitrogen_dioxide,// Concentration of Nitrogen Dioxide in µg/m³
        nitrogen_monoxide,// Concentration of Nitrogen Monoxide in ppb, µg/m³
        nitrous_oxide,// Concentration of Nitrous Oxide in µg/m³
        ozone,// Concentration of Ozone in ppb or µg/m³
        ph,// Potential hydrogen (pH) value of a water solution
        pm1,// Concentration of particulate matter less than 1 micrometer in µg/m³
        pm25,// Concentration of particulate matter less than 2.5 micrometers in µg/m³
        pm4,// Concentration of particulate matter less than 4 micrometers in µg/m³
        pm10,// Concentration of particulate matter less than 10 micrometers in µg/m³
        power_factor,// Power factor (unitless), unit may be None or %
        power,// Power in mW, W, kW, MW, GW or TW
        precipitation,// Accumulated precipitation in cm, in or mm
        precipitation_intensity,// Precipitation intensity in in/d, in/h, mm/d or mm/h
        pressure,// Pressure in mPa, Pa, hPa, kPa, bar, cbar, mbar, mmHg, inHg, inH₂O or psi
        reactive_energy,// Reactive energy in varh or kvarh
        reactive_power,// Reactive power in mvar, var, or kvar
        signal_strength,// Signal strength in dB or dBm
        sound_pressure,// Sound pressure in dB or dBA
        speed,// Generic speed in ft/s, in/d, in/h, in/s, km/h, kn, m/s, mph, mm/d, or mm/s
        sulphur_dioxide,// Concentration of sulphur dioxide in ppb or µg/m³
        temperature,// Temperature in °C, °F or K
        temperature_delta,// Temperature difference between two measurements in °C, °F, or K
        timestamp,// Datetime object or timestamp string (ISO 8601)
        volatile_organic_compounds,// Concentration of volatile organic compounds in µg/m³ or mg/m³
        volatile_organic_compounds_parts,// Ratio of volatile organic compounds in ppm or ppb
        voltage,// Voltage in V, mV, µV, kV, MV
        volume,// Generic volume in L, mL, gal, fl. oz., m³, ft³, CCF, or MCF
        volume_flow_rate,// Volume flow rate in m³/h, m³/min, m³/s, ft³/min, L/h, L/min, L/s, gal/d, gal/h, gal/min, or mL/s
        volume_storage,// Generic stored volume in L, mL, gal, fl. oz., m³, ft³, CCF, or MCF
        water,// Water consumption in L, gal, m³, ft³, CCF, or MCF
        weight,// Generic mass in kg, g, mg, µg, oz, lb, or st
        wind_direction,// Wind direction in °
        wind_speed,// Wind speed in Beaufort, ft/s, km/h, kn, m/s, or mph
    }
}
